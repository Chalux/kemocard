using cfg.character;
using cfg.pawn;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class BaseHealCard : BaseCardScript
{
    public sealed override void UseCard(BaseBattleCard parent)
    {
        var mod = GameCore.ControllerMgr.GetControllerModel<BattleModel>(ControllerType.Battle);
        HealStruct heal = new()
        {
            Value = parent.RealTimeValue,
            Role = parent.User.Role,
            Attribute = parent.Attribute,
            Target = parent.Target,
            User = parent.User,
        };
        CustomizeHeal(ref heal, parent);
        mod?.DoHeal(heal);
    }

    protected virtual void CustomizeHeal(ref HealStruct heal, BaseBattleCard parent)
    {
    }
}