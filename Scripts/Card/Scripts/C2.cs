using cfg.character;
using cfg.pawn;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class C2 : BaseAttackCard
{
    protected override void CustomizeDamage(ref Damage damage, BaseBattleCard parent)
    {
        if (parent.Target.Count <= 0) return;
        var isSpecial = (parent.Target[0].Attribute & (int)Attribute.FIRE) > 0;
        damage.Times = isSpecial ? 2 : 1;
    }
}