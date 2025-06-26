using System.Collections.Generic;
using System.Linq;
using cfg.card;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Card;

public partial class BaseBattleCard(BaseCard card) : BaseCard(card.Id)
{
    public BattleCharacter User;
    public BasePawn Target;
    public int RealTimeValue;
    public int RealTimeChain = 1;

    protected virtual void UseCard()
    {
    }

    // 暴露给外部使用
    public void UseCardExpose()
    {
        UseCard();
        User.UsedCardThisTurn.Add(this);
        User.UsedCardThisBattle.Add(this);
    }

    public virtual void UpdateRealTimeValue()
    {
        int result = Value;
        if (User == null)
        {
            RealTimeValue = result;
            return;
        }

        if (Tags.Contains(Tag.PATTACK))
        {
            result += User.PAttack;
        }
        else if (Tags.Contains(Tag.MATTACK))
        {
            result += User.MAttack;
        }
        else if (Tags.Contains(Tag.HEAL))
        {
            result += User.Heal;
        }

        var model = GameCore.ControllerMgr.GetControllerModel<BattleModel>((int)ControllerType.Battle);
        if (model is { IsInBattle: true })
        {
            var teamList = new List<BattleCharacter> { User };
            foreach (var character in model.Teammates.Where(character =>
                         !teamList.Contains(character) && character.TempUsedCard.Any(baseBattleCard =>
                             (baseBattleCard.Attribute & Attribute) != 0)))
            {
                teamList.Add(character);
            }

            RealTimeChain = teamList.Count;
        }

        var chainRate = RealTimeChain switch
        {
            2 => 1.2f,
            3 => 1.5f,
            4 => 1.9f,
            _ => 1
        };

        RealTimeValue = (int)(result * chainRate);
    }
}