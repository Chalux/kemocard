using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Loading;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;

namespace kemocard.Scripts.Module.GameUI;

public partial class MenuScene : BaseView
{
    [Export] private Button _quitButton;
    [Export] private Button _settingButton;
    [Export] private Button _startButton;

    protected override void OnReady()
    {
        _startButton.Pressed += StartButtonOnPressed;
        _settingButton.Pressed += SettingButtonOnPressed;
        _quitButton.Pressed += () =>
        {
            AlertView.ShowAlert(new AlertViewData { Message = "是否退出游戏？", agreeCallback = AgreeCallback });
        };
    }

    private void AgreeCallback()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
        GetTree().Quit();
    }

    private void SettingButtonOnPressed()
    {
        UpdateEvent(CommonEvent.OpenSettingScene);
    }

    private void StartButtonOnPressed()
    {
        GameCore.ViewMgr.CloseView(ViewId);

        GameCore.CardMgr.ClearPool();
        GameCore.CardMgr.InitPool();

        var controller = GameCore.ControllerMgr.GetModule<RunController>(ControllerType.Run);
        if (FileAccess.FileExists(RunModel.RunSavePath))
        {
            controller.ResetModel();
            controller.Load();
        }
        else
        {
            controller.AddCharacter("1");
        }

        var model =
            new LoadingModel(GameCore.ControllerMgr.GetModule<LoadingController>(ControllerType.Loading))
            {
                SceneName = "GameScene",
                ViewType = ViewType.GameScene
            };

        GameCore.ControllerMgr.SendUpdate(ControllerType.Loading, CommonEvent.ShowLoadingScene, model);
    }
}