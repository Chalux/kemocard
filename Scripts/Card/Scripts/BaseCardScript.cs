using Godot;

namespace kemocard.Scripts.Card.Scripts;

public partial class BaseCardScript : GodotObject
{
    public virtual void UseCard(BaseBattleCard parent)
    {
    }

    public virtual void UpdateRealTimeValue(BaseBattleCard parent, ref float result)
    {
    }
}