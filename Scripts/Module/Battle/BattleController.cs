using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.Controller;

namespace kemocard.Scripts.Module.Battle;

public class BattleController : BaseController
{
    public BattleSystem BattleSystem = new();
    public BattleController() : base()
    {
        GameCore.ViewMgr.Register(ViewType.BattleView, new ViewInfo()
        {
            ViewName = "BattleView",
            ViewType = ViewType.BattleView,
            Controller = this,
            ResPath = GameCore.GetScenePath("BattleView"),
        });

        BattleModel model = new BattleModel(this);
        SetModel(model);

        InitModuleEvent();
    }

    public override void InitModuleEvent()
    {
        base.InitModuleEvent();
        RegisterEvent(CommonEvent.StartBattle, OnBattleBegin);
        RegisterEvent(CommonEvent.BattleEvent_UseCard, OnBattleEventUseCard);
        RegisterEvent(CommonEvent.BattleEvent_CancelUseCard, OnBattleEventCancelUseCard);
        RegisterEvent(CommonEvent.BattleEvent_StartTurn, OnBattleEventStartTurn);
    }

    private void OnBattleEventStartTurn(object[] obj)
    {
        (Model as BattleModel)?.OnTurnStart();
    }

    private void OnBattleEventCancelUseCard(object[] obj)
    {
        (Model as BattleModel)?.OnCancelBattleUseCard(obj[0] as BaseBattleCard);
    }

    private void OnBattleEventUseCard(object[] obj)
    {
        (Model as BattleModel)?.OnBattleUseCard(obj[0] as BaseBattleCard);
    }

    private void OnBattleBegin(object[] obj)
    {
        var model = GetModel<BattleModel>();
        if (model == null) return;
        model.Init();
        GameCore.ViewMgr.OpenView(ViewType.BattleView, obj);
    }
}