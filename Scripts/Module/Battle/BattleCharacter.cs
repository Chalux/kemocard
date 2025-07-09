using System.Collections.Generic;
using System.Linq;
using kemocard.Scripts.Buff;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.Battle;

public class BattleCharacter : BaseCharacter, IBattlePawn
{
    int IBattlePawn.MaxHealth
    {
        get => MaxHealth;
        set => MaxHealth = value;
    }

    public int CurrentHealth { get; set; }

    public List<BaseBattleCard> Deck = [];

    public Dictionary<int, BaseBattleCard> Hand = new()
    {
        { 0, null },
        { 1, null },
        { 2, null },
        { 3, null },
        { 4, null },
    };

    public List<string> Discard = [];

    public List<BaseBattleCard> TempUsedCard = [];

    public const int MaxHandSize = 5;

    public bool IsDead { get; set; }

    int IBattlePawn.PDefense
    {
        get => PDefense;
        set => PDefense = value;
    }

    int IBattlePawn.MDefense
    {
        get => MDefense;
        set => MDefense = value;
    }

    public bool IsResetDeck;

    public int Cost;
    public int CanUseCost;
    public int CurrLockedCost;

    public bool IsConfirm;

    public List<BaseBattleCard> UsedCardThisTurn = [];
    public List<BaseBattleCard> UsedCardThisBattle = [];

    public BattleCharacter(BaseCharacter character)
    {
        if (character == null) return;
        Id = character.Id;
        InitFromConfig(Id);
        Role = character.Role;
        Cards = character.Cards;
    }

    public int GetHandCount()
    {
        return Hand.Values.Count(card => card != null);
    }

    public void Init()
    {
        CurrentHealth = MaxHealth;
        foreach (var baseCard in Cards)
        {
            Deck.Add(new BaseBattleCard(baseCard));
        }

        // 洗牌
        StaticUtil.Shuffle(Deck);
    }

    public void AddToHand(BaseBattleCard card)
    {
        // 找到Hand中为null的位置添加进去
        for (var i = 0; i < Hand.Count; i++)
        {
            if (Hand[i] != null) continue;
            Hand[i] = card;
            break;
        }
    }

    public void DrawCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (GetHandCount() >= MaxHandSize)
            {
                return;
            }

            if (Deck.Count == 0 && IsResetDeck == false)
            {
                StaticUtil.Shuffle(Discard);
                foreach (var se in Discard)
                {
                    // 创建新的卡牌变体加到卡组里
                    Deck.Add(new BaseBattleCard(new BaseCard(se)));
                }

                Discard.Clear();
                IsResetDeck = true;
            }

            if (Deck.Count == 0)
            {
                return;
            }

            AddToHand(Deck[0]);
            Deck.RemoveAt(0);
        }
    }

    public void UseCard(string id, List<BasePawn> target)
    {
        if (string.IsNullOrWhiteSpace(id) || target == null) return;
        var card = Hand.Values.FirstOrDefault(baseBattleCard => baseBattleCard.Id == id);
        if (card == null) return;
        card.Target = target;
        card.User = this;
        card.UpdateRealTimeValue();
        TempUsedCard.Add(card);
        UpdateCanUseCost();
        GameCore.ControllerMgr.SendUpdate(ControllerType.Battle, CommonEvent.BattleEvent_UseCard, card);
    }

    public void CancelUseCard(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        var card = Hand.Values.FirstOrDefault(baseBattleCard => baseBattleCard.Id == id);
        if (card == null) return;
        card.Target = null;
        card.User = null;
        card.UpdateRealTimeValue();
        TempUsedCard.Remove(card);
        UpdateCanUseCost();
        GameCore.ControllerMgr.SendUpdate(ControllerType.Battle, CommonEvent.BattleEvent_CancelUseCard, card);
    }

    private void UpdateCanUseCost()
    {
        CanUseCost = Cost - TempUsedCard.Sum(x => x.Cost);
    }
}