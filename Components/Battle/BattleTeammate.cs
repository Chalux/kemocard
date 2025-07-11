using System;
using System.Linq;
using cfg.card;
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
    [Export] private Button _selectTeammateBtn;
    public BattleCharacter BattleHero { get; private set; }
    public Action OnClick;

    public override void _Ready()
    {
        base._Ready();
        GameCore.EventBus.AddEvent(this, CommonEvent.PlayerPropUpdate, RefreshItem);
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_Render, RefreshItem);
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_SelectCardChanged, OnSelectCardChanged);
        _usedCard.RenderHandler = RenderHandler;
        _selectTeammateBtn.Pressed += SelectTeammateBtnOnPressed;
        RefreshItem();
        MouseEntered += ShowHint;
        MouseExited += StaticUtil.HideHint;
    }

    private void SelectTeammateBtnOnPressed()
    {
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_TeammateItemClicked, this);
    }

    private void RenderHandler(Control arg1, int arg2, object arg3)
    {
        if (arg1 is CardBigItem card)
        {
            card.CustomMinimumSize = new(192, 288);
            var battleCard = arg3 as BaseBattleCard;
            card.Init(battleCard);
            card.SetValue(battleCard?.RealTimeValue ?? 0);
        }
    }

    public override void _ExitTree()
    {
        GameCore.EventBus.RemoveEvent(this, CommonEvent.PlayerPropUpdate, RefreshItem);
        GameCore.EventBus.RemoveEvent(this, CommonEvent.BattleEvent_Render, RefreshItem);
        GameCore.EventBus.RemoveEvent(this, CommonEvent.BattleEvent_SelectCardChanged, OnSelectCardChanged);
        _selectTeammateBtn.Pressed -= SelectTeammateBtnOnPressed;
        MouseEntered -= ShowHint;
        MouseExited -= StaticUtil.HideHint;
        base._ExitTree();
    }

    public void SetBattleHero(BattleCharacter battleHero)
    {
        if (BattleHero == battleHero) return;
        BattleHero = battleHero;
        Visible = BattleHero != null;
        RefreshItem();
    }

    public void RefreshItem(object o = null)
    {
        if (BattleHero == null || string.IsNullOrWhiteSpace(BattleHero.Id))
        {
            Visible = false;
            return;
        }

        if (o != null && (o is not BattleCharacter character || character.Id != BattleHero.Id))
        {
            return;
        }

        _icon.Texture = FileAccess.FileExists(BattleHero.Icon)
            ? ResourceLoader.Load<CompressedTexture2D>(BattleHero.Icon)
            : null;
        SetCost(BattleHero.CanUseCost, BattleHero.Cost);
        _usedCard.SetData(BattleHero.TempUsedCard.ToList<object>());
        _hpBar.MaxValue = BattleHero.MaxHealth;
        _hpBar.Value = BattleHero.CurrentHealth;
    }

    public void SetCost(int canUseCost, int cost)
    {
        _costLab.Text = $"{canUseCost}/{cost}";
    }

    private void OnSelectCardChanged(object o)
    {
        _selectTeammateBtn.Visible = o is BattleCardItem { Card.TargetType: TargetType.ST };
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        if (@event is InputEventMouseButton mb && mb.IsPressed() && (mb.ButtonMask & MouseButtonMask.Left) > 0)
        {
            OnClick.Invoke();
        }
    }

    public void ShowHint()
    {
        StaticUtil.ShowHint(BattleHero.GetDetailDesc());
    }
}