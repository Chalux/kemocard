using cfg.character;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class BaseCardScript : GodotObject
{
    /**
     * Mod可能为null，使用时注意
     */
    protected BattleModel Mod = GameCore.ControllerMgr.GetControllerModel<BattleModel>(ControllerType.Battle);

    public virtual void UseCard(BaseBattleCard parent)
    {
    }

    public virtual void UpdateRealTimeValue(BaseBattleCard parent, ref float result)
    {
    }

    protected Damage CreateDamageFromOwner(BaseBattleCard parent)
    {
        Damage damage = new()
        {
            User = parent.User,
            Target = parent.Target,
            Role = parent.User.Role,
            Attribute = parent.Attribute,
            Tags = parent.Tags,
            Value = parent.RealTimeValue,
        };
        return damage;
    }
}