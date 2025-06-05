using System;

namespace kemocard.Scripts.Timer;

public class GameTimerData
{
    private double _timer;
    private Action _callback;

    public GameTimerData(double inTimer, Action inCallback)
    {
        _timer = inTimer;
        _callback = inCallback;
    }

    public bool OnUpdate(double dt)
    {
        _timer -= dt;
        if (!(_timer <= 0)) return false;
        _callback.Invoke();
        return true;
    }

    public void Clear()
    {
        _callback = null;
    }
}