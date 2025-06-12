using System.Collections.Generic;
using System.Linq;
using cfg.card;
using cfg.character;
using cfg.pawn;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC.Controller;
using kemocard.Scripts.MVC.Model;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.Battle;

public class BattleModel(BaseController inController) : BaseModel(inController)
{
    public bool IsInBattle { get; private set; } = false;
    public List<BattleCharacter> Teammates = [];
    public List<BattleEnemy> Enemies = [];
    public BattlePhase BattlePhase { get; private set; } = BattlePhase.None;
    public readonly PriorityQueue<EnemyAction, int> EnemyActionQueue = new();

    public override void Init()
    {
        base.Init();
        IsInBattle = false;
        foreach (var battleCharacter in Teammates)
        {
            battleCharacter.Dispose();
        }

        Teammates = [];
        foreach (var battleEnemy in Enemies)
        {
            battleEnemy.Dispose();
        }

        Enemies = [];
        BattlePhase = BattlePhase.None;
        EnemyActionQueue.Clear();
    }

    public void OnBattleStart(List<BaseCharacter> team, List<BasePawn> enemies)
    {
        Init();
        IsInBattle = true;
        BattlePhase = BattlePhase.BattleStart;
        foreach (var battleCharacter in team.Select(baseCharacter => new BattleCharacter(baseCharacter)))
        {
            battleCharacter.Cost = 3;
            Teammates.Add(battleCharacter);
        }

        Teammates.Sort((characterA, battleCharacterB) => characterA.Role - battleCharacterB.Role);

        foreach (var battleEnemy in enemies.Select(basePawn => new BattleEnemy(basePawn)))
        {
            Enemies.Add(battleEnemy);
        }
        
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_StartBattle_Ready, null);

        foreach (var battleCharacter in Teammates)
        {
            battleCharacter.ExecuteBuffs();
        }

        foreach (var basePawn in Enemies)
        {
            basePawn.ExecuteBuffs();
        }
    }

    public void OnBattleUseCard(BaseBattleCard card)
    {
        if (BattlePhase != BattlePhase.TurnAction) return;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_Render, null);
    }

    public void OnCancelBattleUseCard(BaseBattleCard card)
    {
        if (BattlePhase != BattlePhase.TurnAction) return;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_Render, null);
    }

    public void OnTurnStart()
    {
        BattlePhase = BattlePhase.TurnStart;
        foreach (var character in Teammates)
        {
            character.DrawCard(character.Draw);
            character.Cost += 1;
            character.CanUseCost = character.Cost;
            character.IsConfirm = false;
        }

        foreach (var battleCharacter in Teammates)
        {
            battleCharacter.ExecuteBuffs();
            battleCharacter.UsedCardThisTurn.Clear();
        }

        foreach (var enemy in Enemies)
        {
            enemy.ExecuteBuffs();
        }

        BattlePhase = BattlePhase.TurnAction;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_Render, null);
    }

    public void OnTurnEnd()
    {
        BattlePhase = BattlePhase.UseCard;
        List<BaseBattleCard> useCardList = new();
        foreach (var battleCharacter in Teammates)
        {
            useCardList = useCardList.Concat(battleCharacter.TempUsedCard).ToList();
        }

        if (useCardList.Count > 0)
        {
            useCardList.Sort((x, y) => x.Sort.CompareTo(y.Sort));
            var thisCard = useCardList[0];
            useCardList.RemoveAt(0);
            while (thisCard != null)
            {
                thisCard.UseCardExpose();
                thisCard = useCardList.Count > 0 ? useCardList[0] : null;
                if (thisCard != null)
                {
                    useCardList.RemoveAt(0);
                }
            }
        }

        BattlePhase = BattlePhase.TurnEnd;
        foreach (var battleCharacter in Teammates)
        {
            battleCharacter.ExecuteBuffs();
        }

        foreach (var battleEnemy in Enemies)
        {
            battleEnemy.ExecuteBuffs();
        }

        BattlePhase = BattlePhase.EnemyAction;
        EnemyActionQueue.Clear();
        foreach (var battleEnemy in Enemies)
        {
            battleEnemy.Action();
        }

        while (EnemyActionQueue.Count > 0)
        {
            var item = EnemyActionQueue.Dequeue();
            item.Action?.Invoke(item);
        }
    }

    public void OnBattleEnd(bool isWin = false)
    {
        Init();
    }

    public void DoDamage(Damage damage)
    {
        if (!IsInBattle) return;
        List<BasePawn> pawns = [..Teammates, ..Enemies];
        var target = pawns.Find(pawn => pawn == damage.Target);
        var user = pawns.Find(pawn => pawn == damage.User);
        if (user == null || target == null) return;
        if (user is BattleCharacter && target is BattleEnemy { CurrentHealth: 0 })
        {
            target = Enemies.Find(enemy => enemy.CurrentHealth > 0);
            if (target == null)
            {
                OnBattleEnd(true);
                return;
            }
        }
        else if (user is BattleEnemy && target is BattleCharacter { CurrentHealth: 0 })
        {
            target = Teammates.Find(c => c.CurrentHealth > 0);
            if (target == null)
            {
                OnBattleEnd(false);
                return;
            }
        }

        user.OnAttack(damage);
        StaticUtil.CalculateAttributeRate(damage);
        target.OnAttacked(damage);

        if (target is BattleCharacter character)
        {
            character.CurrentHealth -= damage.Value;
            if (character.CurrentHealth <= 0)
            {
                character.IsDead = true;
                character.CurrentHealth = 0;
                CheckBattleEnd();
            }
        }
        else if (target is BattleEnemy enemy)
        {
            enemy.CurrentHealth -= damage.Value;
            if (enemy.CurrentHealth <= 0)
            {
                enemy.IsDead = true;
                enemy.CurrentHealth = 0;
                CheckBattleEnd();
            }
        }
    }

    public void CheckBattleEnd()
    {
        if (Enemies.All(enemy => enemy.IsDead || enemy.CurrentHealth <= 0))
        {
            OnBattleEnd(true);
        }
        else if (Teammates.All(character => character.IsDead || character.CurrentHealth <= 0))
        {
            OnBattleEnd(false);
        }
    }
}

public record struct Damage
{
    public int Value;
    public Role Role;
    public int Attribute;
    public HashSet<Tag> Tags;
    public BasePawn Target;
    public BasePawn User;
}

public enum BattlePhase
{
    None,
    BattleStart,
    TurnStart,
    TurnAction,
    UseCard,
    TurnEnd,
    EnemyAction,
}