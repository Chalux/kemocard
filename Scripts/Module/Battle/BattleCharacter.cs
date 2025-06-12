using System.Collections.Generic;
using System.Linq;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.Battle;

public partial class BattleCharacter : BaseCharacter
{
    public int CurrentHealth = 0;

    public List<BaseBattleCard> Deck = new();
    public List<BaseBattleCard> Hand = new();
    public List<string> Discard = new();

    public List<BaseBattleCard> TempUsedCard = new();

    public const int MaxHandSize = 5;

    public bool IsDead = false;
    public bool IsResetDeck = false;

    public int Cost = 0;
    public int CanUseCost = 0;
    public int CurrLockedCost = 0;

    public bool IsConfirm = false;

    public List<BaseBattleCard> UsedCardThisTurn = new();
    public List<BaseBattleCard> UsedCardThisBattle = new();

    public BattleCharacter(BaseCharacter hero)
    {
        CurrentHealth = MaxHealth;
        foreach (var baseCard in Cards)
        {
            Deck.Add(new BaseBattleCard(baseCard));
        }

        // 洗牌
        StaticUtil.Shuffle(Deck);
    }

    public void DrawCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (Hand.Count >= MaxHandSize)
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

            Hand.Add(Deck[0]);
            Deck.RemoveAt(0);
        }
    }

    public void UseCard(string id, BasePawn target)
    {
        var card = Hand.Find(card => card.Id == id);
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
        var card = Hand.Find(card => card.Id == id);
        if (card == null) return;
        card.Target = null;
        card.User = null;
        card.UpdateRealTimeValue();
        TempUsedCard.Remove(card);
        UpdateCanUseCost();
        GameCore.ControllerMgr.SendUpdate(ControllerType.Battle, CommonEvent.BattleEvent_CancelUseCard, card);
    }

    public void ExecuteBuffs()
    {
        foreach (var keyValuePair in Buffs)
        {
            keyValuePair.Value?.ApplyBuff();
        }
    }

    private void UpdateCanUseCost()
    {
        CanUseCost = Cost - TempUsedCard.Sum(x => x.Cost);
    }
}