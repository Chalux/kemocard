using Godot;
using kemocard.Components.List;
using kemocard.Scripts.Common;
using kemocard.Scripts.Pawn;

namespace kemocard.Components.Hero;

public partial class HeroItem : Control, ISelectableItem
{
    private BaseCharacter _hero;
    [Export] private TextureRect _icon;
    public BaseCharacter Hero
    {
        get => _hero;
        set
        {
            _hero = value;
            RefreshItem();
        }
    }

    public override void _Ready()
    {
        base._Ready();
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    public override void _ExitTree()
    {
        MouseEntered -= OnMouseEntered;
        MouseExited -= OnMouseExited;
        base._ExitTree();
    }

    public void SetHero(BaseCharacter hero)
    {
        _hero = hero;
        RefreshItem();
    }

    public void SetHeroById(string id)
    {
        _hero = new BaseCharacter();
        _hero.InitFromConfig(id);
        RefreshItem();
    }

    private void OnMouseExited()
    {
        StaticUtil.HideHint();
    }

    private void OnMouseEntered()
    {
        if (_hero == null) return;
        StaticUtil.ShowHint(_hero.GetDetailDesc());
    }

    public void RefreshItem()
    {
        if (_hero == null) return;
        if (FileAccess.FileExists(_hero.Icon)) _icon.Texture = ResourceLoader.Load<CompressedTexture2D>(_hero.Icon);
    }

    public int Index { get; set; }
    public VirtualList List { get; set; }
}