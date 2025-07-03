using System.Collections.Generic;
using System.Linq;
using Godot;
using kemocard.Components.List;
using kemocard.Components.Reward;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.GameUI;

public partial class GameScene : BaseView
{
    [Export] private Button _teamEditBtn;
    [Export] private Button _quitBtn;
    [Export] private Button _giveUpBtn;
    [Export] private Control _debugControl;
    [Export] private Button _debugFightBtn;
    [Export] private TextEdit _debugFightEdit;
    [Export] private VirtualList _unhandledRewardList;

    public override void DoShow(params object[] args)
    {
        base.DoShow();
        _teamEditBtn.Pressed += TeamEditBtnOnPressed;
        _quitBtn.Pressed += QuitBtnOnPressed;
        _giveUpBtn.Pressed += GiveUpBtnOnPressed;
        _debugControl.Visible = OS.IsDebugBuild();
        if (_debugControl.Visible)
        {
            _debugFightBtn.Pressed += DebugFightBtnOnPressed;
        }

        _unhandledRewardList.RenderHandler = RenderHandler;

        GameCore.EventBus.AddEvent(this, CommonEvent.UnhandledRewardChanged, OnUnhandledRewardChanged);
        var mod = GameCore.ControllerMgr.GetControllerModel<RunModel>(ControllerType.Run);
        if (mod != null) _unhandledRewardList.SetData(mod.UnhandledRewards.ToList<object>());
    }

    public override void DoClose(params object[] args)
    {
        _teamEditBtn.Pressed -= TeamEditBtnOnPressed;
        _quitBtn.Pressed -= QuitBtnOnPressed;
        _giveUpBtn.Pressed -= GiveUpBtnOnPressed;
        _debugFightBtn.Pressed -= DebugFightBtnOnPressed;
        _unhandledRewardList.RenderHandler = null;
        base.DoClose();
    }

    private void TeamEditBtnOnPressed()
    {
        GameCore.ViewMgr.OpenView(ViewType.TeamEditView);
    }

    private void GiveUpBtnOnPressed()
    {
        DirAccess.RemoveAbsolute(RunModel.RunSavePath);
        GameCore.ControllerMgr.GetModule<RunController>(ControllerType.Run).ResetModel();
        GameCore.ViewMgr.OpenView(ViewType.MenuScene);
        GameCore.ViewMgr.CloseView(ViewId);
    }

    private void QuitBtnOnPressed()
    {
        GameCore.ControllerMgr.GetModule<RunController>(ControllerType.Run).Save();
        GameCore.ViewMgr.OpenView(ViewType.MenuScene);
        GameCore.ViewMgr.CloseView(ViewId);
    }

    private void DebugFightBtnOnPressed()
    {
        List<BasePawn> list = [];
        var ids = _debugFightEdit.Text.Split(",");
        // foreach (var id in ids)
        // {
        //     if (string.IsNullOrWhiteSpace(id)) continue;
        //     var pawn = new BasePawn();
        //     pawn.InitFromConfig(id);
        //     list.Add(pawn);
        // }

        if (ids.Length > 0)
            GameCore.ControllerMgr.SendUpdate(ControllerType.Battle, CommonEvent.StartBattle_BY_PRESET, ids[0]);
    }

    private void OnUnhandledRewardChanged(object arg)
    {
        var mod = GameCore.ControllerMgr.GetControllerModel<RunModel>(ControllerType.Run);
        if (mod == null)
        {
            _unhandledRewardList.SetData([]);
        }
        else
        {
            _unhandledRewardList.SetData(mod.UnhandledRewards.ToList<object>());
        }
    }

    private void RenderHandler(Control arg1, int arg2, object arg3)
    {
        if (arg1 is RewardComponent rc)
        {
            rc.Init(arg3 as string);
        }
    }
}