namespace kemocard.Scripts.Common;

public class Singleton<T> where T : new()
{
    public static T Instance { get; } = new();

    public virtual void Init()
    {
    }

    public virtual void OnDispose()
    {
    }

    public virtual void Update(double deltaTime)
    {
    }
}