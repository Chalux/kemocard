using System.Collections.Generic;
using System.Linq;
using cfg.card;
using Godot;
using kemocard.Scripts.Card.Scripts;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Card;

public partial class BaseBattleCard : BaseCard
{
    public BattleCharacter User;
    public List<BasePawn> Target;
    public int RealTimeValue;
    public int RealTimeChain = 1;
    public BaseCardScript Script;

    public BaseBattleCard(BaseCard card) : base(card.Id)
    {
        var path = StaticUtil.GetCardScriptPath(card.Id);
        if (!FileAccess.FileExists(path)) return;
        var res = ResourceLoader.Load<CSharpScript>(path);
        Script = res?.New().As<BaseCardScript>();
    }

    public BaseBattleCard() : base(null)
    {
    }

    protected void UseCard()
    {
        Script?.UseCard(this);
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
        if (User == null)
        {
            RealTimeValue = Value;
            return;
        }

        float result = Value;
        float costRate = CostRate.GetValueOrDefault(Cost, 1f);

        if (Tags.Contains(Tag.PATTACK))
        {
            result += User.PAttack * costRate;
        }
        else if (Tags.Contains(Tag.MATTACK))
        {
            result += User.MAttack * costRate;
        }
        else if (Tags.Contains(Tag.HEAL))
        {
            result += User.Heal * costRate;
        }

        Script.UpdateRealTimeValue(this, ref result);

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

        RealTimeValue = (int)(result * ChainRate.GetValueOrDefault(RealTimeChain, 1f));
    }

    public static Dictionary<int, float> ChainRate = new()
    {
        [2] = 1.2f,
        [3] = 1.5f,
        [4] = 1.9f,
    };

    public static Dictionary<int, float> CostRate = new()
    {
        [0] = 0.15f,
        [1] = 0.4f,
        [2] = 0.7f,
        [3] = 1f,
        [4] = 1.7f,
        [5] = 2.5f,
    };
}