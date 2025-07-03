using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.Module.Game;
using kemocard.Scripts.Module.GameUI;
using kemocard.Scripts.Module.Loading;
using kemocard.Scripts.Module.Run;

namespace kemocard.Scripts.MVC;

public static class ModuleFactory
{
    public static void RegisterModule()
    {
        GameCore.ControllerMgr.Register(ControllerType.GameUIController, new GameUIController());
        GameCore.ControllerMgr.Register(ControllerType.Game, new GameController());
        GameCore.ControllerMgr.Register(ControllerType.Loading, new LoadingController());
        GameCore.ControllerMgr.Register(ControllerType.Battle, new BattleController());
        GameCore.ControllerMgr.Register(ControllerType.Run, new RunController());
    }

    public static void InitModule()
    {
        //手动在工厂中初始化模块，主要是为了有模块的初始化顺序
        GameCore.ControllerMgr.InitModule(ControllerType.GameUIController);
        GameCore.ControllerMgr.InitModule(ControllerType.Game);
        GameCore.ControllerMgr.InitModule(ControllerType.Loading);
        GameCore.ControllerMgr.InitModule(ControllerType.Run);
        GameCore.ControllerMgr.InitModule(ControllerType.Battle);
    }
}

public enum ControllerType
{
    GameUIController,
    Game,
    Loading,
    Battle,
    Run,
}