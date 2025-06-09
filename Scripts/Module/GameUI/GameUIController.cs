using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.Controller;

namespace kemocard.Scripts.Module.GameUI;

public class GameUIController : BaseController
{
    public GameUIController()
    {
        GameCore.ViewMgr.Register(ViewType.MenuScene, new ViewInfo
        {
            ViewName = "MenuScene",
            ResPath = GameCore.GetScenePath("MenuScene"),
            Controller = this,
            ViewType = ViewType.MenuScene
        });

        GameCore.ViewMgr.Register(ViewType.SettingScene, new ViewInfo
        {
            ViewName = "SettingScene",
            ResPath = GameCore.GetScenePath("SettingScene"),
            Controller = this,
            ViewType = ViewType.SettingScene,
        });

        GameCore.ViewMgr.Register(ViewType.AlertView, new ViewInfo
        {
            ViewName = "AlertView",
            ResPath = GameCore.GetScenePath("AlertView"),
            Controller = this,
            ViewType = ViewType.AlertView
        });
        
        GameCore.ViewMgr.Register(ViewType.CompendiumScene, new ViewInfo
        {
            ViewName = "CompendiumScene",
            ResPath = GameCore.GetScenePath("CompendiumScene"),
            Controller = this,
            ViewType = ViewType.CompendiumScene
        });

        InitModuleEvent();
        InitGlobalEvent();
    }

    public override void InitModuleEvent()
    {
        RegisterEvent(CommonEvent.OpenMenuScene, OpenMenuScene);
        RegisterEvent(CommonEvent.OpenSettingScene, OpenSettingScene);
        RegisterEvent(CommonEvent.OpenAlertView, OpenAlertView);
    }

    private void OpenAlertView(params object[] obj)
    {
        GameCore.ViewMgr.OpenView(ViewType.AlertView, obj);
    }

    private void OpenMenuScene(params object[] args)
    {
        GameCore.ViewMgr.OpenView(ViewType.MenuScene, args);
    }

    private void OpenSettingScene(params object[] args)
    {
        GameCore.ViewMgr.OpenView(ViewType.SettingScene, args);
    }
}