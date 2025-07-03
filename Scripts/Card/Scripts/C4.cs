using cfg.character;
using cfg.pawn;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class C4 : BaseCardScript
{
    public override void UseCard(BaseBattleCard parent)
    {
        var mod = GameCore.ControllerMgr.GetControllerModel<BattleModel>(ControllerType.Battle);
        if (mod == null) return;
        Damage damage = new()
        {
            User = parent.User,
            Target = parent.Target,
            Role = Role.ATTACKER,
            Attribute = (int)Attribute.WATER,
            Tags = parent.Tags,
            Value = parent.RealTimeValue,
            Times = 3,
        };
        mod.DoDamage(damage);
    }
}