using cfg.character;
using cfg.pawn;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class C12 : BaseCardScript
{
    public override void UseCard(BaseBattleCard parent)
    {
        var mod = GameCore.ControllerMgr.GetControllerModel<BattleModel>(ControllerType.Battle);
        HealStruct heal = new()
        {
            Value = parent.RealTimeValue,
            Role = Role.SUPPORT,
            Attribute = (int)Attribute.LIGHT,
            Target = parent.Target,
            User = parent.User,
        };
        mod?.DoHeal(heal);
    }
}