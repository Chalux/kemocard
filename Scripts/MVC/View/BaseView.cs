using Godot;
using Godot.Collections;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC.Controller;

namespace kemocard.Scripts.MVC.View;

public partial class BaseView : Control, IBaseView
{
    private bool _isInit;
    protected CanvasItem Canvas;
    protected Dictionary<string, Node> MCacheGos = new();

    public bool IsInit()
    {
        return _isInit;
    }

    public bool IsShow()
    {
        return Visible;
    }

    public virtual void InitUI()
    {
    }

    public virtual void InitData()
    {
        _isInit = true;
    }

    public virtual void DoShow(params object[] args)
    {
    }

    public virtual void DoClose(params object[] args)
    {
        SetVisibility(false);
        GameCore.EventBus.RemoveObjAllEvents(this);
    }

    public void DestroyView()
    {
        Controller = null;
        QueueFree();
    }

    public void UpdateEvent(string eventName, params object[] args)
    {
        Controller.SendUpdate(eventName, args);
    }

    public void UpdateControllerEvent(int controllerKey, string eventName, params object[] args)
    {
        BaseController.SendControllerUpdate(controllerKey, eventName, args);
    }

    public void SetVisibility(bool isVisible)
    {
        Visible = isVisible;
    }

    public int ViewId { get; set; }
    public BaseController Controller { get; set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        OnEnterTree();
    }

    protected virtual void OnEnterTree()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        OnReady();
    }

    protected virtual void OnReady()
    {
    }

    public Node FindNode(string key)
    {
        if (MCacheGos.TryGetValue(key, out var node)) return node;

        MCacheGos.Add(key, FindChild(key, false));
        return MCacheGos[key];
    }

    public T FindNode<T>(string key) where T : Node
    {
        return FindNode(key) as T;
    }

    public virtual void Close()
    {
        GameCore.ViewMgr.CloseView(ViewId);
    }
}