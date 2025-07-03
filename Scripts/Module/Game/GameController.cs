using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.Controller;

namespace kemocard.Scripts.Module.Game;

public class GameController : BaseController
{
    public GameController()
    {
        GameCore.ViewMgr.Register(ViewType.GameScene, new ViewInfo
        {
            Controller = this,
            ResPath = GameCore.GetScenePath("GameScene"),
            ViewName = "GameScene",
            ViewType = ViewType.GameScene,
        });
    }

    public override void Init()
    {
        SendControllerUpdate(ControllerType.GameUIController, CommonEvent.OpenMenuScene);
    }
}