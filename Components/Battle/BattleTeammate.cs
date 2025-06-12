using System.Linq;
using Godot;
using kemocard.Components.Card;
using kemocard.Components.List;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;

namespace kemocard.Components.Battle;

public partial class BattleTeammate : Control
{
    [Export] private TextureRect _icon;
    [Export] private VirtualList _usedCard;
    [Export] private TextureProgressBar _hpBar;
    [Export] private Label _costLab;
    private BattleCharacter _battleHero = null;

    public override void _Ready()
    {
        base._Ready();
        VisibilityChanged += OnVisibilityChanged;
        _usedCard.RenderHandler = RenderHandler;
    }

    private void RenderHandler(Control arg1, int arg2, object arg3)
    {
        if (arg1 is CardBigItem card)
        {
            card.Init(arg3 as BaseBattleCard);
        }
    }

    public override void _ExitTree()
    {
        VisibilityChanged -= OnVisibilityChanged;
        GameCore.EventBus.RemoveEvent(this, CommonEvent.PlayerPropUpdate, RefreshItem);
        base._ExitTree();
    }

    private void OnVisibilityChanged()
    {
        if (Visible)
        {
            GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_Render, RefreshItem);
        }
        else
        {
            GameCore.EventBus.RemoveEvent(this, CommonEvent.BattleEvent_Render, RefreshItem);
        }
    }

    public void SetBattleHero(BattleCharacter battleHero)
    {
        if (_battleHero == battleHero) return;
        _battleHero = battleHero;
        RefreshItem();
    }

    public void RefreshItem(object o = null)
    {
        if (_battleHero == null) return;

        _icon.Texture = FileAccess.FileExists(_battleHero.Icon)
            ? ResourceLoader.Load<CompressedTexture2D>(_battleHero.Icon)
            : null;
        SetCost(_battleHero.CanUseCost, _battleHero.Cost);
        _usedCard.SetData(_battleHero.TempUsedCard.ToList<object>());
        _hpBar.MaxValue = _battleHero.MaxHealth;
        _hpBar.Value = _battleHero.CurrentHealth;
    }

    public void SetCost(int canUseCost, int cost)
    {
        _costLab.Text = $"{canUseCost}/{cost}";
    }
}