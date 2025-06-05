using System;
using System.Collections.Generic;

namespace kemocard.Scripts.Timer;

public class GameTimer
{
    private List<GameTimerData> _timerList = new();

    public void Register(float inTimer, Action inCallback)
    {
        _timerList.Add(new GameTimerData(inTimer, inCallback));
    }

    public void OnUpdate(double dt)
    {
        for (int i = _timerList.Count - 1; i > 0; i--)
        {
            if (_timerList[i].OnUpdate(dt))
            {
                _timerList.RemoveAt(i);
            }
        }
    }

    public void Break()
    {
        foreach (var gameTimerData in _timerList)
        {
            gameTimerData.Clear();
        }

        _timerList.Clear();
    }

    public int Count()
    {
        return _timerList.Count;
    }
}