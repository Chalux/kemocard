using Godot;

namespace kemocard.Scripts.Common;

public partial class KemoCamera : Camera2D
{
    private Vector2 _originGlobalPos;

    public Vector2 OriginGlobalPos
    {
        get => _originGlobalPos;
        private set => _originGlobalPos = value;
    }

    public override void _Ready()
    {
        base._Ready();
        OriginGlobalPos = GlobalPosition;
    }
}