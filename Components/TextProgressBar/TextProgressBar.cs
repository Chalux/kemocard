using Godot;

namespace kemocard.Components.TextProgressBar;

public partial class TextProgressBar : TextureProgressBar
{
    [Export] public Label Lab;

    public override void _Ready()
    {
        base._Ready();
        Changed += SetText;
        ValueChanged += OnValueChanged;
    }

    public override void _ExitTree()
    {
        Changed -= SetText;
        ValueChanged -= OnValueChanged;
        base._ExitTree();
    }

    private void OnValueChanged(double value)
    {
        SetText();
    }
    
    private void SetText()
    {
        if (Lab != null)
        {
            Lab.Text = $"{Value}/{MaxValue}";
        }
    }
}