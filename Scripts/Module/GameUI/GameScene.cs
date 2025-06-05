using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;

namespace kemocard.Scripts.Module.GameUI;

public partial class GameScene : BaseView
{
    [Export] private Button _teamEditBtn;
    [Export] private Button _quitBtn;
    [Export] private Button _giveUpBtn;

    public override void DoShow(params object[] args)
    {
        base.DoShow();
        _teamEditBtn.Pressed += TeamEditBtnOnPressed;
        _quitBtn.Pressed += QuitBtnOnPressed;
        _giveUpBtn.Pressed += GiveUpBtnOnPressed;
    }

    public override void DoClose(params object[] args)
    {
        _teamEditBtn.Pressed -= TeamEditBtnOnPressed;
        _quitBtn.Pressed -= QuitBtnOnPressed;
        _giveUpBtn.Pressed -= GiveUpBtnOnPressed;
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
}