using Godot;

namespace kemocard.Scripts;

public partial class BaseKemoTileMapLayer : TileMapLayer
{
    public static Vector2I LeftNormal = new Vector2I(-1, 1);
    public static Vector2I LeftUpNormal = new Vector2I(-1, 0);
    public static Vector2I LeftDownNormal = new Vector2I(0, 1);
    public static Vector2I RightNormal = new(1, -1);
    public static Vector2I RightUpNormal = new Vector2I(0, -1);
    public static Vector2I RightDownNormal = new Vector2I(1, 0);
    
    public Vector2I CurrentCoord = Vector2I.Zero;

    public override void _Ready()
    {
        base._Ready();
        CurrentCoord = Vector2I.Zero;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion mouseMotion)
        {
            CurrentCoord = LocalToMap(mouseMotion.Position);
        }
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        base._UnhandledKeyInput(@event);
        if (@event.IsActionPressed("Confirm"))
        {
            GD.Print(CurrentCoord);
        }
    }
}