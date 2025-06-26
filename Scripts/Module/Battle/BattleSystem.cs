using kemocard.Scripts.Common;

namespace kemocard.Scripts.Module.Battle;

public class BattleSystem
{
    public void StartBattle()
    {
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_StartBattle);
    }

    public void StartTurn()
    {
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_StartTurn);
    }

    public void EndAction()
    {
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_EndAction);
    }

    public void EndTurn()
    {
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_EndTurn);
        StartTurn();
    }
}