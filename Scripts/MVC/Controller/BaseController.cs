using System;
using System.Collections.Generic;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC.Model;
using kemocard.Scripts.MVC.View;

namespace kemocard.Scripts.MVC.Controller;

public class BaseController
{
    private readonly Dictionary<string, Action<object[]>> _eventMap = new();

    protected BaseModel Model;

    public virtual void Init()
    {
    }

    public virtual void OnLoadView(IBaseView view)
    {
    }

    public virtual void OpenView(IBaseView view)
    {
    }

    public virtual void CloseView(IBaseView view)
    {
    }

    public void RegisterEvent(string eventName, Action<object[]> action)
    {
        if (!_eventMap.TryAdd(eventName, action)) _eventMap[eventName] += action;
    }

    public void UnRegisterEvent(string eventName)
    {
        _eventMap.Remove(eventName);
    }

    public void SendUpdate(string eventName, params object[] args)
    {
        if (_eventMap.TryGetValue(eventName, out var action))
            action.Invoke(args);
        else
            GD.PushError($"event {eventName} not found, In {GetType().Name}");
    }

    public static void SendControllerUpdate(int controllerKey, string eventName, params object[] args)
    {
        GameCore.ControllerMgr.SendUpdate(controllerKey, eventName, args);
    }

    public static void SendControllerUpdate(ControllerType controllerType, string eventName, params object[] args)
    {
        GameCore.ControllerMgr.SendUpdate((int)controllerType, eventName, args);
    }

    public void SetModel(BaseModel inModel)
    {
        Model = inModel;
        Model.Controller = this;
    }

    public T GetModel<T>() where T : BaseModel
    {
        return Model as T;
    }

    public static T GetControllerModel<T>(int controllerKey) where T : BaseModel
    {
        return GameCore.ControllerMgr.GetControllerModel<T>(controllerKey);
    }

    public virtual void Destroy()
    {
        RemoveModuleEvent();
        RemoveGlobalEvent();
    }

    public virtual void InitModuleEvent()
    {
    }

    public virtual void RemoveModuleEvent()
    {
    }

    public virtual void InitGlobalEvent()
    {
    }

    public virtual void RemoveGlobalEvent()
    {
    }
}