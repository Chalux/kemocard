using Godot;

namespace kemocard.Components.TextProgressBar;

public partial class TextProgressBar : TextureProgressBar
{
    [Export] public Label Lab;

    public override void _Ready()
    {
        base._Ready();
        Changed += OnValueChanged;
    }

    public override void _ExitTree()
    {
        Changed -= OnValueChanged;
        base._ExitTree();
    }

    private void OnValueChanged()
    {
        Lab.Text = $"{Value}";
    }
}