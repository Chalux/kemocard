using Godot;

namespace kemocard.Scripts.Module.Battle;

public interface IBattlePawnBase
{
    public virtual bool Update(double dt)
    {
        return false;
    }
}