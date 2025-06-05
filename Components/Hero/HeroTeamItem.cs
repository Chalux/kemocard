using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.Pawn;

namespace kemocard.Components.Hero;

public partial class HeroTeamItem : Control
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
        if (hero == null) return;
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
        NameLab.Text = _hero.Name;
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
}