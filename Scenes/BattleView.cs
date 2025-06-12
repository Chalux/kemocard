using System.Linq;
using Godot;
using kemocard.Components.Battle;
using kemocard.Components.TextProgressBar;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;

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
    [Export] private Control _monsterControl;
    private BattleCharacter _currTeammate;
    private BattleModel _mod;
    private BattleController _controller;

    public override void DoShow(params object[] args)
    {
        base.DoShow(args);
        _controller = GameCore.ControllerMgr.GetModule<BattleController>(ControllerType.Battle);
        _mod = _controller.GetModel<BattleModel>();
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_StartBattle_Ready, OnBattleStartReady);
        GameCore.EventBus.AddEvent(this, CommonEvent.BattleEvent_Render, Render);
        _confirmBtn.Pressed += ConfirmBtnOnPressed;
    }

    private void ConfirmBtnOnPressed()
    {
        if (_mod.Teammates.All((c) => c.IsConfirm == true))
        {
            _controller.BattleSystem.EndTurn();
        }
    }

    private void Render(object obj)
    {
    }

    public override void DoClose(params object[] args)
    {
        GameCore.EventBus.RemoveEvent(this, CommonEvent.BattleEvent_StartBattle_Ready, OnBattleStartReady);
        GameCore.EventBus.RemoveEvent(this, CommonEvent.BattleEvent_Render, Render);
        _confirmBtn.Pressed -= ConfirmBtnOnPressed;
        base.DoClose(args);
    }

    private void OnBattleStartReady(object obj)
    {
        SetTeammateByIndex(0);
    }

    private void SetTeammateByIndex(int index)
    {
        switch (index)
        {
            case 0:
                _currTeammate = _mod.Teammates[0];
                _teammate1.SetBattleHero(_mod.Teammates[1]);
                _teammate2.SetBattleHero(_mod.Teammates[2]);
                _teammate3.SetBattleHero(_mod.Teammates[3]);
                break;
            case 1:
                _currTeammate = _mod.Teammates[1];
                _teammate1.SetBattleHero(_mod.Teammates[0]);
                _teammate2.SetBattleHero(_mod.Teammates[2]);
                _teammate3.SetBattleHero(_mod.Teammates[3]);
                break;
            case 2:
                _currTeammate = _mod.Teammates[2];
                _teammate1.SetBattleHero(_mod.Teammates[0]);
                _teammate2.SetBattleHero(_mod.Teammates[1]);
                _teammate3.SetBattleHero(_mod.Teammates[3]);
                break;
            case 3:
                _currTeammate = _mod.Teammates[3];
                _teammate1.SetBattleHero(_mod.Teammates[0]);
                _teammate2.SetBattleHero(_mod.Teammates[1]);
                _teammate3.SetBattleHero(_mod.Teammates[2]);
                break;
        }
    }
}