using System.Collections.Generic;
using kemocard.Scripts.MVC.Controller;
using kemocard.Scripts.MVC.Model;

namespace kemocard.Scripts.MVC;

public class ControllerMgr
{
    private readonly Dictionary<int, BaseController> _modules = new();

    public void Register(ControllerType controllerType, BaseController controller)
    {
        Register((int)controllerType, controller);
    }

    public void Register(int controllerKey, BaseController controller)
    {
        _modules.TryAdd(controllerKey, controller);
    }

    public void UnRegister(int controllerKey)
    {
        _modules.Remove(controllerKey);
    }

    public T GetModule<T>(ControllerType type) where T : BaseController
    {
        return GetModule(type) as T;
    }

    private BaseController GetModule(ControllerType type)
    {
        return GetModule((int)type);
    }

    private BaseController GetModule(int controllerKey)
    {
        return _modules.GetValueOrDefault(controllerKey);
    }

    public void Clear()
    {
        _modules.Clear();
    }

    public void ClearModules()
    {
        foreach (var modulesKey in _modules.Keys)
        {
            _modules[modulesKey].Destroy();
            _modules.Remove(modulesKey);
        }
    }

    public void SendUpdate(ControllerType controllerType, string eventName, params object[] args)
    {
        SendUpdate((int)controllerType, eventName, args);
    }

    public void SendUpdate(int controllerKey, string eventName, params object[] args)
    {
        if (_modules.TryGetValue(controllerKey, out var controller)) controller.SendUpdate(eventName, args);
    }

    public T GetControllerModel<T>(int controllerKey) where T : BaseModel
    {
        return _modules.TryGetValue(controllerKey, out var controller) ? controller.GetModel<T>() : null;
    }

    public T GetControllerModel<T>(ControllerType controllerType) where T : BaseModel
    {
        return GetControllerModel<T>((int)controllerType);
    }

    public void InitModule(int controllerKey)
    {
        _modules.TryGetValue(controllerKey, out var controller);
        controller?.Init();
        controller?.InitGlobalEvent();
        controller?.InitModuleEvent();
    }

    public void InitModule(ControllerType controllerType)
    {
        InitModule((int)controllerType);
    }
}