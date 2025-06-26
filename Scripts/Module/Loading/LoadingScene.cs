using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;

namespace kemocard.Scripts.Module.Loading;

public partial class LoadingScene : BaseView
{
    private double _countTime;
    [Export] private TextureRect _loadingImage;

    public override void DoShow(params object[] args)
    {
        _countTime = 0;
        ProcessMode = ProcessModeEnum.Inherit;
    }

    public override void DoClose(params object[] args)
    {
        ProcessMode = ProcessModeEnum.Disabled;
        base.DoClose(args);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _countTime += delta;
        _loadingImage.SetRotation((float)_countTime * 2);
        var model = Controller.GetModel<LoadingModel>();
        var status = ResourceLoader.LoadThreadedGetStatus(GameCore.GetScenePath(model.SceneName));
        if (status == ResourceLoader.ThreadLoadStatus.Failed)
        {
            GameCore.ViewMgr.CloseView(ViewType.LoadingScene);
        }

        if (status == ResourceLoader.ThreadLoadStatus.Loaded && _countTime >= 1)
        {
            if (ResourceLoader.LoadThreadedGet(GameCore.GetScenePath(model.SceneName)) is PackedScene resource)
            {
                model.LoadingCallback?.Invoke();
                GameCore.ViewMgr.CloseView(ViewType.LoadingScene);
                GameCore.ViewMgr.OpenView(model.ViewType);
            }
        }
    }
}