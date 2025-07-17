using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class BaseAttackCard : BaseCardScript
{
    public sealed override void UseCard(BaseBattleCard parent)
    {
        var damage = CreateDamageFromOwner(parent);
        CustomizeDamage(ref damage, parent);
        Mod?.DoDamage(damage);
    }

    protected virtual void CustomizeDamage(ref Damage damage, BaseBattleCard parent)
    {
    }
}