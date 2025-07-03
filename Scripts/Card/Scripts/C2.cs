using cfg.character;
using cfg.pawn;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class C2 : BaseCardScript
{
    public override void UseCard(BaseBattleCard parent)
    {
        var mod = GameCore.ControllerMgr.GetControllerModel<BattleModel>(ControllerType.Battle);
        if (mod == null) return;
        if (parent.Target.Count <= 0) return;
        var isSpecial = (parent.Target[0].Attribute & (int)Attribute.FIRE) > 0;
        Damage damage = new()
        {
            User = parent.User,
            Target = parent.Target,
            Role = Role.ATTACKER,
            Attribute = (int)Attribute.WATER,
            Tags = parent.Tags,
            Value = parent.RealTimeValue,
            Times = isSpecial ? 2 : 1,
        };
        mod.DoDamage(damage);
    }
}