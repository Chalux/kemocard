using System.Collections.Generic;
using kemocard.Scripts.Common;

namespace kemocard.Scripts.Module.Battle;

public class BattleSystem
{
    private IBattlePawnBase _currentPawn;
    private uint turnNum = 0;

    public IBattlePawnBase CurrentPawn
    {
        get => _currentPawn;
        private set { _currentPawn = value; }
    }

    public void StartBattle()
    {
        turnNum = 0;
        GameCore.EventBus.PostEvent(CommonEvent.BattleEvent_StartBattle, null);
    }

    public void StartTurn()
    {
        turnNum++;
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

    public List<IBattlePawnBase> StartPawns;
    public List<IBattlePawnBase> CurrentAlivePawns;
    public List<IBattlePawnBase> DeadPawns;

    public void StartBattle(List<IBattlePawnBase> initPawnList)
    {
        
    }
}