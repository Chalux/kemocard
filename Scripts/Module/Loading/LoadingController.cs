using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.Controller;

namespace kemocard.Scripts.Module.Loading;

public class LoadingController : BaseController
{
    public LoadingController()
    {
        base.Init();
        GameCore.ViewMgr.Register(ViewType.LoadingScene, new ViewInfo
        {
            Controller = this,
            ResPath = GameCore.GetScenePath("LoadingScene"),
            ViewName = "LoadingScene",
            ViewType = ViewType.LoadingScene,
        });
    }

    public override void InitModuleEvent()
    {
        RegisterEvent(CommonEvent.ShowLoadingScene, LoadingSceneCallback);
    }

    private void LoadingSceneCallback(params object[] args)
    {
        LoadingModel model = args[0] as LoadingModel;
        if (model == null)
        {
            GameCore.ViewMgr.CloseView(ViewType.LoadingScene);
            if (args[1] is ViewType viewType) GameCore.ViewMgr.OpenView(viewType);
            return;
        }

        SetModel(model);

        GameCore.ViewMgr.OpenView(ViewType.LoadingScene);

        Error error = ResourceLoader.LoadThreadedRequest(GameCore.GetScenePath(model.SceneName));
        if (error != Error.Ok)
        {
            GameCore.ViewMgr.CloseView(ViewType.LoadingScene);
            if (args[1] is ViewType viewType) GameCore.ViewMgr.OpenView(viewType);
        }
    }
}