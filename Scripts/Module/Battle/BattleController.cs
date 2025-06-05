using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.Controller;

namespace kemocard.Scripts.Module.Battle;

public class BattleController : BaseController
{
    public BattleController() : base()
    {
        GameCore.ViewMgr.Register(ViewType.BattleView, new ViewInfo()
        {
            ViewName = "BattleView",
            ViewType = ViewType.BattleView,
            Controller = this,
            ResPath = GameCore.GetScenePath("BattleView"),
        });

        InitModuleEvent();
    }

    public override void InitModuleEvent()
    {
        base.InitModuleEvent();
        RegisterEvent(CommonEvent.StartBattle, OnBattleBegin);
    }

    private void OnBattleBegin(object[] obj)
    {
        
    }
}