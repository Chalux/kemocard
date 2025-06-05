using System;

namespace kemocard.Scripts.Timer;

public class TimerMgr
{
    private GameTimer timer = new();

    public void Register(float time, Action callback)
    {
        timer.Register(time, callback);
    }

    public void OnUpdate(double dt)
    {
        timer.OnUpdate(dt);
    }
}