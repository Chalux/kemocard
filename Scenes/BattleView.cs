using System.Linq;
using cfg.card;
using Godot;
using kemocard.Components.Battle;
using kemocard.Components.Card;
using kemocard.Components.Enemy;
using kemocard.Components.List;
using kemocard.Components.TextProgressBar;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;
using kemocard.Scripts.Pawn;

namespace kemocard.Scenes;

public partial class BattleView : BaseView
{
    [Export] private BattleTeammate _teammate1;
    [Export] private BattleTeammate _teammate2;
    [Export] private BattleTeammate _teammate3;
    [Export] private TextureRect _currIcon;
    [Export] private TextProgressBar _currHpBar;
    [Export] private Button _confirmBtn;
    [Export] private Label _currCostLab;
    [Export] private VirtualList _cardList;
    [Export] private Control _debugControl;
    [Export] private Button _debugEndBattle;
    private BattleCharacter _currTeammate;
    private BattleModel _mod;
    private BattleController _controller;
    private BattleCardItem _currCardItem;
    [Export] private Control _enemyControl;
    [Export] private Button _selectAllTeamBtn;
    [Export] private Button _selectAllEnemyBtn;
    [Export] private Button _selectSelfBtn;

    public override void DoShow(params object[] args)
    {
        base.DoShow(args);
        _controller = GameCore.ControllerMgr.GetModule<BattleController>(ControllerType.Battle);
        _mod = _controller.GetModel<BattleModel>();
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_StartBattle_Ready, OnBattleStartReady);
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_Render, Render);
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_EndBattle, OnBattleEnd);
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_EnemyItemClicked, OnEnemyItemClicked);
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_TeammateItemClicked, OnTeammateItemClicked);
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_SelectCardChanged, OnSelectCardChanged);
        _confirmBtn.Pressed += ConfirmBtnOnPressed;
        _selectSelfBtn.Pressed += SelectSelfBtnOnPressed;
        _selectAllTeamBtn.Pressed += SelectAllTeamBtnOnPressed;
        _selectAllEnemyBtn.Pressed += SelectAllEnemyBtnOnPressed;
        _debugControl.Visible = OS.IsDebugBuild();
        if (_debugControl.Visible)
        {
            _debugEndBattle.Pressed += DebugEndBattleOnPressed;
        }

        _cardList.RenderHandler = RenderHandler;
        _cardList.SelectedHandler = SelectedHandler;
        _cardList.CanSelectHandler = CanSelectHandler;
    }

    private bool CanSelectHandler(int index)
    {
        var cell = _cardList.GetCell<BattleCardItem>(index);
        if (cell == null || _currTeammate == null) return false;
        if (cell.Status == BattleCardStatus.Confirmed) return !_currTeammate.IsConfirm;
        var cost = _currTeammate.CanUseCost;
        return !_currTeammate.IsConfirm && cell.Card.Cost <= cost;
    }

    private void SelectedHandler(int newIndex, int previousIndex)
    {
        if (previousIndex == newIndex)
        {
            _currCardItem?.SetStatus(BattleCardStatus.Normal);
            // var card = _currTeammate.TempUsedCard.Find(card => card.Id == _currCardItem.Card.Id);
            // _currTeammate.UpdateCanUseCost();
            // _currTeammate.TempUsedCard.Remove(card);
            _currTeammate.CancelUseCard(_currCardItem?.Card.Id);
            _cardList.SelectedIndex = -1;
        }
        else
        {
            if (newIndex == -1)
            {
                _currCardItem = null;
            }
            else
            {
                var temp = _cardList.GetCell<BattleCardItem>(newIndex);
                if (temp.Status == BattleCardStatus.Confirmed)
                {
                    _currTeammate?.CancelUseCard(temp.Card.Id);
                    temp.SetStatus(BattleCardStatus.Normal);
                    _currCardItem = null;
                }
                else
                {
                    _currCardItem = temp;
                }
            }

            GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_SelectCardChanged, _currCardItem);
        }
    }

    private void RenderHandler(Control arg1, int arg2, object arg3)
    {
        if (arg1 is BattleCardItem cardItem)
        {
            cardItem.SetCustomMinimumSize(new(192, 288));
            var card = arg3 as BaseBattleCard;
            cardItem.Init(card);
            BattleCardStatus status;
            if (_currCardItem == cardItem)
            {
                status = BattleCardStatus.Targeting;
            }
            else if (_currTeammate?.TempUsedCard.Contains(cardItem.Card) ?? false)
            {
                status = BattleCardStatus.Confirmed;
            }
            else
            {
                status = BattleCardStatus.Normal;
            }

            cardItem.SetStatus(status);
        }
    }

    private void OnBattleEnd(object obj)
    {
        Close();
    }

    private void DebugEndBattleOnPressed()
    {
        _mod.OnBattleEnd();
    }

    private void ConfirmBtnOnPressed()
    {
        if (_currTeammate.IsConfirm == false)
        {
            _currTeammate.IsConfirm = true;
            _confirmBtn.Text = "已确定";
            if (_mod.Teammates.All(c => c.IsConfirm))
            {
                _currCardItem = null;
                _controller.SendUpdate(CommonEvent.BattleEvent_EndTurn);
            }
        }
        else
        {
            _currTeammate.IsConfirm = false;
            _confirmBtn.Text = "确定";
        }
    }

    private void Render(object obj = null)
    {
        if (_currTeammate != null)
        {
            _currIcon.Texture = FileAccess.FileExists(_currTeammate.Icon)
                ? ResourceLoader.Load<CompressedTexture2D>(_currTeammate.Icon)
                : null;
            _currCostLab.Text = $"{_currTeammate.CanUseCost}/{_currTeammate.Cost}";
            _currHpBar.MaxValue = _currTeammate.MaxHealth;
            _currHpBar.Value = _currTeammate.CurrentHealth;
            _cardList.SetData(_currTeammate.Hand.ToList<object>());
            _confirmBtn.Text = _currTeammate.IsConfirm ? "已确定" : "确定";
        }
        else
        {
            _currIcon.Texture = null;
            _currCostLab.Text = "";
            _currHpBar.MaxValue = 0;
            _currHpBar.Value = 0;
            _cardList.SetData([]);
            _confirmBtn.Text = "确定";
        }
    }

    public override void DoClose(params object[] args)
    {
        _confirmBtn.Pressed -= ConfirmBtnOnPressed;
        _debugEndBattle.Pressed -= DebugEndBattleOnPressed;
        _selectSelfBtn.Pressed -= SelectSelfBtnOnPressed;
        _cardList.RenderHandler = null;
        _cardList.SelectedHandler = null;
        base.DoClose(args);
    }

    private void OnBattleStartReady(object obj)
    {
        var idx = _mod.Teammates.FindIndex(character =>
            character != null && !string.IsNullOrWhiteSpace(character.Id));
        SetTeammateByIndex(idx);
        var res = ResourceLoader.Load<PackedScene>("res://Components/Enemy/EnemyItem.tscn");
        foreach (var battleEnemy in _mod.Enemies)
        {
            var enemyItem = res.Instantiate<EnemyItem>();
            enemyItem.Init(battleEnemy);
            _enemyControl.AddChild(enemyItem);
        }

        Render();
    }

    private void SetTeammateByIndex(int index)
    {
        switch (index)
        {
            case 0:
                _currTeammate = _mod.Teammates[0];
                _teammate1.SetBattleHero(_mod.Teammates.Count <= 1 ? null : _mod.Teammates[1]);
                _teammate2.SetBattleHero(_mod.Teammates.Count <= 2 ? null : _mod.Teammates[2]);
                _teammate3.SetBattleHero(_mod.Teammates.Count <= 3 ? null : _mod.Teammates[3]);
                break;
            case 1:
                _currTeammate = _mod.Teammates[1];
                _teammate1.SetBattleHero(_mod.Teammates[0]);
                _teammate2.SetBattleHero(_mod.Teammates.Count <= 2 ? null : _mod.Teammates[2]);
                _teammate3.SetBattleHero(_mod.Teammates.Count <= 3 ? null : _mod.Teammates[3]);
                break;
            case 2:
                _currTeammate = _mod.Teammates[2];
                _teammate1.SetBattleHero(_mod.Teammates[0]);
                _teammate2.SetBattleHero(_mod.Teammates[1]);
                _teammate3.SetBattleHero(_mod.Teammates.Count <= 3 ? null : _mod.Teammates[3]);
                break;
            case 3:
                _currTeammate = _mod.Teammates[3];
                _teammate1.SetBattleHero(_mod.Teammates[0]);
                _teammate2.SetBattleHero(_mod.Teammates[1]);
                _teammate3.SetBattleHero(_mod.Teammates[2]);
                break;
        }

        _confirmBtn.Text = _currTeammate.IsConfirm ? "已确定" : "确定";
    }

    private void OnEnemyItemClicked(object o)
    {
        if (_currCardItem is { Card.TargetType: TargetType.SE } && o is EnemyItem enemyItem)
        {
            // var currCard = _currTeammate.Hand.Find(card => card.Id == _currCardItem.Card.Id);
            // if (currCard != null)
            // {
            //     currCard.Target = enemyItem.EnemyData;
            // }
            _currTeammate.UseCard(_currCardItem.Card.Id, [enemyItem.EnemyData]);
            _currCardItem.SetStatus(BattleCardStatus.Confirmed);
            _currCardItem = null;
            GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_SelectCardChanged, _currCardItem);
        }
    }

    private void OnTeammateItemClicked(object o)
    {
        if (_currCardItem is { Card.TargetType: TargetType.ST } && o is BattleTeammate teammateItem)
        {
            _currTeammate.UseCard(_currCardItem.Card.Id, [teammateItem.BattleHero]);
            _currCardItem.SetStatus(BattleCardStatus.Confirmed);
            _currCardItem = null;
            GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_SelectCardChanged, _currCardItem);
        }
    }

    private void SelectSelfBtnOnPressed()
    {
        if (_currCardItem is not { Card.TargetType: TargetType.SELF }) return;
        _currTeammate.UseCard(_currCardItem.Card.Id, [_currTeammate]);
        _currCardItem.SetStatus(BattleCardStatus.Confirmed);
        _currCardItem = null;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_SelectCardChanged, _currCardItem);
    }

    private void SelectAllTeamBtnOnPressed()
    {
        if (_currCardItem is not { Card.TargetType: TargetType.AT }) return;
        _currTeammate.UseCard(_currCardItem.Card.Id, _mod.Teammates.ToList<BasePawn>());
        _currCardItem.SetStatus(BattleCardStatus.Confirmed);
        _currCardItem = null;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_SelectCardChanged, _currCardItem);
    }

    private void SelectAllEnemyBtnOnPressed()
    {
        if (_currCardItem is not { Card.TargetType: TargetType.AE }) return;
        _currTeammate.UseCard(_currCardItem.Card.Id, _mod.Enemies.ToList<BasePawn>());
        _currCardItem.SetStatus(BattleCardStatus.Confirmed);
        _currCardItem = null;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_SelectCardChanged, _currCardItem);
    }

    private void OnSelectCardChanged(object o)
    {
        _selectAllEnemyBtn.Visible = _selectAllTeamBtn.Visible = _selectSelfBtn.Visible = false;
        if (o is BattleCardItem cardItem)
        {
            switch (cardItem.Card.TargetType)
            {
                case TargetType.AE:
                    _selectAllEnemyBtn.Visible = true;
                    break;
                case TargetType.AT:
                    _selectAllTeamBtn.Visible = true;
                    break;
                case TargetType.SELF:
                    _selectSelfBtn.Visible = true;
                    break;
            }
        }
    }
}