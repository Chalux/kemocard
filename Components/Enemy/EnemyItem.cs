using cfg.card;
using Godot;
using kemocard.Components.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;

namespace kemocard.Components.Enemy;

public partial class EnemyItem : Control
{
    [Export] private TextProgressBar.TextProgressBar _hpBar;
    [Export] private Button _selectEnemyBtn;
    public BattleEnemy EnemyData { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_SelectCardChanged, OnSelectCardChanged);
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_Render, UpdateItem);
        _selectEnemyBtn.Pressed += SelectEnemyBtnOnPressed;
        MouseEntered += OnMouseEntered;
        MouseExited += StaticUtil.HideHint;
    }

    public override void _ExitTree()
    {
        GameCore.EventBus.RemoveEvent(this, CommonEvent.BattleEvent_SelectCardChanged, OnSelectCardChanged);
        GameCore.EventBus.RemoveEvent(this, CommonEvent.BattleEvent_Render, UpdateItem);
        _selectEnemyBtn.Pressed -= SelectEnemyBtnOnPressed;
        MouseEntered -= OnMouseEntered;
        MouseExited -= StaticUtil.HideHint;
        base._ExitTree();
    }

    private void OnSelectCardChanged(object obj)
    {
        _selectEnemyBtn.Visible = obj is BattleCardItem { Card.TargetType: TargetType.SE };
    }

    public void Init(BattleEnemy pawn)
    {
        EnemyData = pawn;
        if (EnemyData != null) SetPosition(EnemyData.Position);
        UpdateItem();
    }

    public void UpdateItem(object data = null)
    {
        if (EnemyData == null) return;
        _hpBar.MaxValue = EnemyData.MaxHealth;
        _hpBar.Value = EnemyData.CurrentHealth;
    }

    private void SelectEnemyBtnOnPressed()
    {
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_EnemyItemClicked, this);
    }

    private void OnMouseEntered()
    {
        StaticUtil.ShowHint(EnemyData.GetEnemyDesc());
    }
}