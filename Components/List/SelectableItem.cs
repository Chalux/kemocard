using Godot;

namespace kemocard.Components.List;

public interface ISelectableItem
{
    public int Index { get; set; }
    public VirtualList List { get; set; }

    public abstract void SetMouseFilter(Control.MouseFilterEnum mouseFilter);

    public void OnGuiInput(InputEvent input)
    {
        if (input is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }) return;
        if (List.CanSelectHandler == null || List.CanSelectHandler.Invoke(Index))
        {
            List.SelectedIndex = Index;
        }
    }
}