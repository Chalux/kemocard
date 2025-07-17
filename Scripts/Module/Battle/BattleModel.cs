using System;
using System.Collections.Generic;
using System.Linq;
using cfg.card;
using cfg.character;
using Godot;
using kemocard.Scripts.Buff;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.Controller;
using kemocard.Scripts.MVC.Model;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.Battle;

public class BattleModel(BaseController inController) : BaseModel(inController)
{
    public bool IsInBattle { get; private set; }
    public bool IsBattleReady { get; private set; }
    private string _presetId;
    public List<BattleCharacter> Teammates = [];
    public List<BattleEnemy> Enemies = [];
    public BattlePhase BattlePhase { get; private set; } = BattlePhase.None;
    public readonly PriorityQueue<EnemyAction, int> EnemyActionQueue = new();
    public int TurnCount { get; private set; }

    public readonly BasePawn System = new()
    {
        Name = "未知",
        Attribute = 0,
        Description = "未知"
    };

    public override void Init()
    {
        base.Init();
        IsInBattle = false;
        IsBattleReady = false;
        _presetId = "";
        Teammates = [];
        Enemies = [];
        BattlePhase = BattlePhase.None;
        EnemyActionQueue.Clear();
    }

    public void OnBattleStart(List<BaseCharacter> team, List<BasePawn> enemies, string presetId)
    {
        Init();
        _presetId = presetId;
        GD.Print("开始战斗");
        IsInBattle = true;
        BattlePhase = BattlePhase.BattleStart;
        foreach (var battleCharacter in from baseCharacter in team
                 where baseCharacter != null
                 select new BattleCharacter(baseCharacter))
        {
            battleCharacter.Init();
            battleCharacter.Cost = 2;
            battleCharacter.DrawCard(5);
            Teammates.Add(battleCharacter);
        }

        Teammates.Sort((characterA, battleCharacterB) => characterA.Role - battleCharacterB.Role);

        foreach (var battleEnemy in enemies.Select(basePawn => new BattleEnemy(basePawn)))
        {
            Enemies.Add(battleEnemy);
        }

        IsBattleReady = true;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_StartBattle_Ready);

        object data = null;
        foreach (var battleCharacter in Teammates)
        {
            battleCharacter.ExecuteBuffs(ref data, BuffTag.BattleStart);
        }

        foreach (var basePawn in Enemies)
        {
            basePawn.ExecuteBuffs(ref data, BuffTag.BattleStart);
        }

        GD.Print("战斗初始化完成，开始第一回合");
        OnTurnStart();
    }

    public void OnBattleUseCard(BaseBattleCard card)
    {
        if (BattlePhase != BattlePhase.TurnAction) return;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_Render);
    }

    public void OnCancelBattleUseCard(BaseBattleCard card)
    {
        if (BattlePhase != BattlePhase.TurnAction) return;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_Render);
    }

    public void OnTurnStart()
    {
        GD.Print("开始新的回合");
        TurnCount++;
        BattlePhase = BattlePhase.TurnStart;
        foreach (var character in Teammates)
        {
            character.DrawCard(character.Draw);
            character.Cost += 1;
            character.CanUseCost = character.Cost;
            character.IsConfirm = false;
        }
        
        foreach (var battleEnemy in Enemies)
        {
            battleEnemy.UpdateActionId();
        }

        GD.Print("抽卡结束，开始执行 buff");
        object data = null;
        foreach (var battleCharacter in Teammates)
        {
            battleCharacter.ExecuteBuffs(ref data, BuffTag.TurnStart);
            battleCharacter.UsedCardThisTurn.Clear();
        }

        foreach (var enemy in Enemies)
        {
            enemy.ExecuteBuffs(ref data, BuffTag.TurnStart);
        }

        GD.Print("执行buff完毕，开始使用卡牌");
        BattlePhase = BattlePhase.TurnAction;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_Render);
    }

    public void OnTurnEnd()
    {
        GD.Print("回合结束，开始使用卡牌");
        BattlePhase = BattlePhase.UseCard;
        List<BaseBattleCard> useCardList = [];
        useCardList = Teammates.Aggregate(useCardList,
            (current, battleCharacter) => current.Concat(battleCharacter.TempUsedCard).ToList());

        if (useCardList.Count > 0)
        {
            useCardList.Sort((x, y) => x.Sort.CompareTo(y.Sort));
            var thisCard = useCardList[0];
            useCardList.RemoveAt(0);
            while (thisCard != null)
            {
                GD.Print($"使用卡牌{thisCard.Name}");
                thisCard.UseCardExpose();
                thisCard = useCardList.Count > 0 ? useCardList[0] : null;
                if (thisCard != null)
                {
                    useCardList.RemoveAt(0);
                }

                if (!IsInBattle)
                {
                    break;
                }
            }
        }

        foreach (var battleCharacter in Teammates)
        {
            battleCharacter.TempUsedCard.Clear();
        }

        if (!IsInBattle) return;

        GD.Print("卡牌使用完毕，执行回合结束的buff");
        BattlePhase = BattlePhase.TurnEnd;
        object data = null;
        foreach (var battleCharacter in Teammates)
        {
            battleCharacter.ExecuteBuffs(ref data, BuffTag.TurnEnd);
        }

        foreach (var battleEnemy in Enemies)
        {
            battleEnemy.ExecuteBuffs(ref data, BuffTag.TurnEnd);
        }

        GD.Print("执行buff完毕，开始执行敌人行为");
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

        GD.Print("执行敌人行为完毕，开始下一回合");
        OnTurnStart();
    }

    public void OnBattleEnd(bool isWin = false)
    {
        if (isWin && !string.IsNullOrWhiteSpace(_presetId))
        {
            var conf = GameCore.Tables.TbBattlePreset.GetOrDefault(_presetId);
            if (conf != null)
            {
                var mod = GameCore.ControllerMgr.GetControllerModel<RunModel>(ControllerType.Run);
                mod?.AddUnhandledReward(conf.Rewards);
            }
        }
        else
        {
            // todo 失败处理
        }

        Init();
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_EndBattle, isWin);
    }

    public void DoDamage(Damage damage)
    {
        if (!IsInBattle) return;
        List<BasePawn> pawns = [..Teammates, ..Enemies];
        var targets = damage.Target;
        var user = damage.User == System ? System : pawns.Find(pawn => pawn == damage.User);
        if (user == null || targets == null || targets.Count <= 0) return;
        switch (user)
        {
            case BattleCharacter { CurrentHealth: 0 } or BattleEnemy { CurrentHealth: 0 }:
                return;
            case BattleCharacter when targets.Count == 1 && targets[0] is BattleEnemy:
            {
                var target = Enemies.Find(enemy => enemy.CurrentHealth > 0);
                if (target == null)
                {
                    OnBattleEnd(true);
                    return;
                }

                targets = [target];
                break;
            }
            case BattleEnemy when targets.Count == 1 && targets[0] is BattleCharacter:
            {
                var target = Teammates.Find(c => c.CurrentHealth > 0);
                if (target == null)
                {
                    OnBattleEnd();
                    return;
                }

                targets = [target];
                break;
            }
        }

        user.OnAttack(ref damage);

        var needCheckBattleEnd = false;
        foreach (var target in targets)
        {
            var tempValue = StaticUtil.CalculateAttributeRate(damage.Value, damage.Attribute, target);
            var tempDamage = damage with { Value = tempValue };
            object data = tempDamage;
            target.ExecuteBuffs(ref data, BuffTag.BeforeAttacked);
            tempDamage.Value = Math.Max(0, tempValue);
            for (var i = 0; i < tempDamage.Times; i++)
            {
                if (target is BattleCharacter character)
                {
                    StaticUtil.ApplyDamage(character, tempDamage);
                    if (character.CurrentHealth <= 0)
                    {
                        character.IsDead = true;
                        character.CurrentHealth = 0;
                        needCheckBattleEnd = true;
                    }
                }
                else if (target is BattleEnemy enemy)
                {
                    StaticUtil.ApplyDamage(enemy, tempDamage);
                    if (enemy.CurrentHealth <= 0)
                    {
                        enemy.IsDead = true;
                        enemy.CurrentHealth = 0;
                        needCheckBattleEnd = true;
                    }
                }
            }

            target.OnAttacked(ref tempDamage);
        }

        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_Render);

        if (needCheckBattleEnd)
        {
            CheckBattleEnd();
        }
    }

    public void DoHeal(HealStruct heal)
    {
        if (!IsInBattle) return;
        var targets = heal.Target;
        var user = heal.User == System ? System : targets.Find(pawn => pawn == heal.User);
        if (user == null || targets is not { Count: > 0 }) return;
        user.OnHeal(ref heal);
        var needCheckBattleEnd = false;
        foreach (var target in targets)
        {
            switch (target)
            {
                case BattleCharacter character:
                    if (!character.IsDead)
                    {
                        character.CurrentHealth += heal.Value;
                        character.OnHealed(heal);
                        if (character.CurrentHealth <= 0)
                        {
                            needCheckBattleEnd = true;
                        }
                    }

                    break;
                case BattleEnemy enemy:
                    if (!enemy.IsDead)
                    {
                        enemy.CurrentHealth += heal.Value;
                        enemy.OnHealed(heal);
                        if (enemy.CurrentHealth <= 0)
                        {
                            needCheckBattleEnd = true;
                        }
                    }

                    break;
            }
        }

        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_Render);

        if (needCheckBattleEnd)
        {
            CheckBattleEnd();
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
            OnBattleEnd();
        }
    }

    public bool IsTaunt(int taunt)
    {
        int maxTaunt = 0;
        foreach (var character in Teammates)
        {
            maxTaunt = Math.Max(maxTaunt, character.Taunt);
        }

        return taunt >= maxTaunt;
    }
}

public record Damage
{
    public int Value = 0;
    public Role Role = Role.NORMAL;
    public int Attribute = 0;
    public HashSet<Tag> Tags = [];
    public List<BasePawn> Target = [];
    public BasePawn User;
    public int Times = 1;
    public Dictionary<DamageModifier, int> Modifiers = new();
    public Action<Damage, BasePawn> OnHit;
    public int ChainNum = 1;
}

public record HealStruct
{
    public int Value = 0;
    public Role Role = Role.NORMAL;
    public int Attribute = 0;
    public List<BasePawn> Target = [];
    public BasePawn User;
    public int ChainNum = 1;
}

public enum DamageModifier
{
    /** 伤害缩放 */
    DamageScale,

    /** 临时防御 */
    TempDefense,
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