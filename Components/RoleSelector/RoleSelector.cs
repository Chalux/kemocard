using System;
using cfg.character;
using Godot;

namespace kemocard.Components.RoleSelector;

public partial class RoleSelector : HBoxContainer
{
    public Role CurrentRole = Role.MAX;
    public Action OnRoleSelected;

    [Export] private Button _max;
    [Export] private Button _attacker;
    [Export] private Button _guard;
    [Export] private Button _blocker;
    [Export] private Button _support;
    [Export] private Button _normal;
    private bool _disableNormal = false;

    public override void _Ready()
    {
        base._Ready();
        SetRole(Role.MAX);
        _max.Pressed += () => SetRole(Role.MAX);
        _attacker.Pressed += () => SetRole(Role.ATTACKER);
        _guard.Pressed += () => SetRole(Role.GUARD);
        _blocker.Pressed += () => SetRole(Role.BLOCKER);
        _support.Pressed += () => SetRole(Role.SUPPORT);
        _normal.Pressed += () => SetRole(Role.NORMAL);
    }

    public override void _ExitTree()
    {
        OnRoleSelected = null;
        base._ExitTree();
    }

    public void DisableNormal(bool newDisable = false)
    {
        _disableNormal = newDisable;
        _normal.Disabled = true;
    }

    public void SetRole(Role role)
    {
        if (_disableNormal && role == Role.NORMAL)
        {
            return;
        }

        ClearColor();
        CurrentRole = role;
        switch (CurrentRole)
        {
            case Role.GUARD:
                _guard.Modulate = Colors.Yellow;
                break;
            case Role.BLOCKER:
                _blocker.Modulate = Colors.Yellow;
                break;
            case Role.ATTACKER:
                _attacker.Modulate = Colors.Yellow;
                break;
            case Role.SUPPORT:
                _support.Modulate = Colors.Yellow;
                break;
            case Role.NORMAL:
                _normal.Modulate = Colors.Yellow;
                break;
            case Role.MAX:
                _max.Modulate = Colors.Yellow;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        OnRoleSelected?.Invoke();
    }

    private void ClearColor()
    {
        _guard.Modulate = _max.Modulate =
            _attacker.Modulate = _blocker.Modulate = _support.Modulate = _normal.Modulate = Colors.White;
    }
}