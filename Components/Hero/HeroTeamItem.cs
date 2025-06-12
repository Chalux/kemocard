using Godot;
using kemocard.Components.List;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;
using kemocard.Scripts.Pawn;

namespace kemocard.Components.Hero;

public partial class HeroTeamItem : Control, ISelectableItem
{
    [Export] public TextureRect Head;
    [Export] public Label NameLab;
    [Export] public Label DeckLab;
    [Export] public Label HealthLab;
    [Export] public Label PAttackLab;
    [Export] public Label MAttackLab;
    [Export] public Label PDefenseLab;
    [Export] public Label MDefenseLab;
    [Export] public Label HealLab;
    private BaseCharacter _hero;
    private bool _click = false;

    public HeroTeamItem()
    {
        SetMouseFilter(MouseFilterEnum.Ignore);
    }

    public void SetHero(BaseCharacter hero)
    {
        if (hero == null)
        {
            Clear();
            return;
        }

        _hero = hero;
        GameCore.EventBus.AddEvent(this, CommonEvent.PlayerPropUpdate, RefreshItem);
        RefreshItem(null);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GameCore.EventBus.RemoveEvent(this, CommonEvent.PlayerPropUpdate, RefreshItem);
    }

    private void RefreshItem(object obj)
    {
        if (_hero == null) return;
        var texture = FileAccess.FileExists(_hero.ImagePath)
            ? ResourceLoader.Load<CompressedTexture2D>(_hero.ImagePath)
            : null;
        if (texture != null) Head.Texture = texture;
        var mod = GameCore.ControllerMgr.GetControllerModel<RunModel>(ControllerType.Run);
        NameLab.Text = _hero.Name + (mod != null && mod.Team[_hero.Role] == _hero ? "已出战" : "");
        DeckLab.Text = _hero.GetDeckDesc();
        HealthLab.Text = _hero.MaxHealth.ToString();
        PAttackLab.Text = _hero.PAttack.ToString();
        MAttackLab.Text = _hero.MAttack.ToString();
        PDefenseLab.Text = _hero.PDefense.ToString();
        MDefenseLab.Text = _hero.MDefense.ToString();
        HealLab.Text = _hero.Heal.ToString();
    }

    public void SetClick(bool newClick)
    {
        _click = newClick;
        SetMouseFilter(_click ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore);
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        if (_click && @event is InputEventMouseButton mb && mb.IsPressed())
        {
            GameCore.ViewMgr.OpenView(ViewType.DeckEditView, _hero.Id);
        }
    }

    public void Clear()
    {
        _hero = null;
        Head.Texture = null;
        NameLab.Text = "";
        DeckLab.Text = "";
        HealthLab.Text = "";
        PAttackLab.Text = "";
        MAttackLab.Text = "";
        PAttackLab.Text = "";
        PDefenseLab.Text = "";
        MDefenseLab.Text = "";
        HealLab.Text = "";
        SetClick(false);
        SetMouseFilter(MouseFilterEnum.Ignore);
        GameCore.EventBus.RemoveEvent(this, CommonEvent.PlayerPropUpdate, RefreshItem);
    }

    public int Index { get; set; }
    public VirtualList List { get; set; }
}