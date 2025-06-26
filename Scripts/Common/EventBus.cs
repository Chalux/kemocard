using System;
using System.Collections.Generic;

namespace kemocard.Scripts.Common;

public class EventBus
{
    /**
     * 事件总线，订阅和分发事件
     */
    public EventBus()
    {
    }

    private readonly Dictionary<string, Action<object>> _eventDict = new();

    /**
     * 临时事件，触发后移除
     */
    private readonly Dictionary<string, Action<object>> _tempEventDict = new();

    /**
     * 对象事件
     */
    private readonly Dictionary<object, Dictionary<string, Action<object>>> _objDict = new();

    public void AddEvent(string eventName, Action<object> action)
    {
        if (_eventDict.TryAdd(eventName, action))
        {
            _eventDict[eventName] += action;
        }
    }

    public void RemoveEvent(string eventName, Action<object> action)
    {
        if (!_eventDict.ContainsKey(eventName)) return;
        _eventDict[eventName] -= action;
        if (_eventDict[eventName] == null)
        {
            _tempEventDict.Remove(eventName);
        }
    }

    public void PostEvent(string eventName, object arg = null)
    {
        if (_eventDict.TryGetValue(eventName, out var value))
            value?.Invoke(arg);
        // 创建 objDict 的副本用于遍历
        var objDictCopy = new Dictionary<object, Dictionary<string, Action<object>>>(_objDict);

        foreach (var keyValuePair in objDictCopy)
        {
            if (keyValuePair.Value.TryGetValue(eventName, out Action<object> eventValue))
            {
                eventValue.Invoke(arg);
            }
        }
    }

    public void AddEvent(object obj, string eventName, Action<object> action)
    {
        if (_objDict.TryGetValue(obj, out var listener))
        {
            if (!listener.TryAdd(eventName, action))
            {
                listener[eventName] += action;
            }
        }
        else
        {
            var tempDict = new Dictionary<string, Action<object>> { { eventName, action } };
            _objDict.Add(obj, tempDict);
        }
    }

    public void RemoveEvent(object obj, string eventName, Action<object> action)
    {
        if (!_objDict.TryGetValue(obj, out var listener)) return;
        if (!listener.ContainsKey(eventName)) return;
        listener[eventName] -= action;
        if (listener[eventName] != null) return;
        listener.Remove(eventName);
        if (_objDict[obj].Count == 0)
        {
            _objDict.Remove(obj);
        }
    }

    public void RemoveObjAllEvents(object obj)
    {
        if (!_objDict.TryGetValue(obj, out var listener)) return;
        listener.Clear();
        _objDict.Remove(obj);
    }

    public void PostEvent(object obj, string eventName, object arg = null)
    {
        if (_objDict.TryGetValue(obj, out var listener))
        {
            listener[eventName]?.Invoke(arg);
        }
    }

    public void AddTempEvent(string eventName, Action<object> action)
    {
        _tempEventDict.TryAdd(eventName, action);
    }

    public void RemoveTempEvent(string eventName, Action<object> action)
    {
        _tempEventDict.Remove(eventName);
    }

    public void PostTempEvent(string eventName, object arg)
    {
        _tempEventDict[eventName]?.Invoke(arg);
        _tempEventDict.Remove(eventName);
    }
}