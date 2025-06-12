using System.Collections.Generic;
using kemocard.Scripts.Common;

namespace kemocard.Scripts.Module.Battle;

public class BattleSystem
{
    public void StartBattle()
    {
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_StartBattle, null);
    }

    public void StartTurn()
    {
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_StartTurn, null);
    }

    public void EndAction()
    {
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_EndAction, null);
    }

    public void EndTurn()
    {
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_EndTurn, null);
        StartTurn();
    }
}