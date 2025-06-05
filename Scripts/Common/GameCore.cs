using cfg;
using kemocard.Scenes;
using kemocard.Scripts.MVC;
using kemocard.Scripts.Timer;

namespace kemocard.Scripts.Common;

public class GameCore : Singleton<GameCore>
{
    public static MainRoot Root;
    public static Tables Tables;
    public static SoundManager SoundMgr { get; private set; }
    public static ControllerMgr ControllerMgr { get; private set; }
    public static ViewManager ViewMgr { get; private set; }
    public static CameraManager CameraMgr { get; private set; }
    public static EventBus EventBus { get; private set; }
    public static TimerMgr TimerMgr { get; private set; }
    public static CardManager CardMgr { get; private set; }

    public override void Init()
    {
        SoundMgr = new SoundManager();
        ControllerMgr = new ControllerMgr();
        ViewMgr = new ViewManager();
        CameraMgr = new CameraManager();
        EventBus = new EventBus();
        TimerMgr = new TimerMgr();
        CardMgr = new CardManager();
    }

    public override void Update(double deltaTime)
    {
        TimerMgr.OnUpdate(deltaTime);
    }

    public static string GetScenePath(string sceneName)
    {
        return $"res://Scenes/{sceneName}.tscn";
    }
}