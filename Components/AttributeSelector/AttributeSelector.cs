using System;
using Godot;
using Attribute = cfg.pawn.Attribute;

namespace kemocard.Components.AttributeSelector;

public partial class AttributeSelector : VBoxContainer
{
    public Attribute CurrentAttribute;
    public Action OnAttributeSelect;
    [Export] private Button _allBtn;
    [Export] private Button _waterBtn;
    [Export] private Button _fireBtn;
    [Export] private Button _windBtn;
    [Export] private Button _earthBtn;
    [Export] private Button _electronBtn;
    [Export] private Button _lightBtn;
    [Export] private Button _darkBtn;

    public override void _Ready()
    {
        base._Ready();
        _allBtn.Pressed += () => SetAttribute(Attribute.NONE);
        _waterBtn.Pressed += () => SetAttribute(Attribute.WATER);
        _fireBtn.Pressed += () => SetAttribute(Attribute.FIRE);
        _windBtn.Pressed += () => SetAttribute(Attribute.WIND);
        _earthBtn.Pressed += () => SetAttribute(Attribute.EARTH);
        _electronBtn.Pressed += () => SetAttribute(Attribute.ELECTRON);
        _lightBtn.Pressed += () => SetAttribute(Attribute.LIGHT);
        _darkBtn.Pressed += () => SetAttribute(Attribute.DARK);
    }

    public void SetAttribute(Attribute attribute)
    {
        CurrentAttribute = attribute;
        OnAttributeSelect?.Invoke();
    }
}