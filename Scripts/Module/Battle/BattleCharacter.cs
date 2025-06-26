using System.Collections.Generic;
using System.Linq;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.Battle;

public class BattleCharacter : BaseCharacter
{
    public int CurrentHealth;

    public List<BaseBattleCard> Deck = [];
    public List<BaseBattleCard> Hand = [];
    public List<string> Discard = [];

    public List<BaseBattleCard> TempUsedCard = [];

    public const int MaxHandSize = 5;

    public bool IsDead;
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
        if (string.IsNullOrWhiteSpace(id) || target == null) return;
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
        if (string.IsNullOrWhiteSpace(id)) return;
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