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

    private Dictionary<string, Action<object>> eventDict = new();

    /**
     * 临时事件，触发后移除
     */
    private Dictionary<string, Action<object>> tempEventDict = new();

    /**
     * 对象事件
     */
    private Dictionary<object, Dictionary<string, Action<object>>> objDict = new();

    public void AddEvent(string eventName, Action<object> action)
    {
        if (eventDict.TryAdd(eventName, action))
        {
            eventDict[eventName] += action;
        }
    }

    public void RemoveEvent(string eventName, Action<object> action)
    {
        if (!eventDict.ContainsKey(eventName)) return;
        eventDict[eventName] -= action;
        if (eventDict[eventName] == null)
        {
            tempEventDict.Remove(eventName);
        }
    }

    public void PostEvent(string eventName, object arg)
    {
        if (eventDict.TryGetValue(eventName, out var value))
            value?.Invoke(arg);
        foreach (var keyValuePair in objDict)
        {
            if (keyValuePair.Value.TryGetValue(eventName, out Action<object> eventValue))
            {
                eventValue.Invoke(arg);
            }
        }
    }

    public void AddEvent(object obj, string eventName, Action<object> action)
    {
        if (objDict.TryGetValue(obj, out var listener))
        {
            if (!listener.TryAdd(eventName, action))
            {
                listener[eventName] += action;
            }
        }
        else
        {
            var tempDict = new Dictionary<string, Action<object>> { { eventName, action } };
            objDict.Add(obj, tempDict);
        }
    }

    public void RemoveEvent(object obj, string eventName, Action<object> action)
    {
        if (!objDict.TryGetValue(obj, out var listener)) return;
        if (!listener.ContainsKey(eventName)) return;
        listener[eventName] -= action;
        if (listener[eventName] != null) return;
        listener.Remove(eventName);
        if (objDict[obj].Count == 0)
        {
            objDict.Remove(obj);
        }
    }

    public void RemoveObjAllEvents(object obj)
    {
        if (!objDict.TryGetValue(obj, out var listener)) return;
        listener.Clear();
        objDict.Remove(obj);
    }

    public void PostEvent(object obj, string eventName, object arg)
    {
        if (objDict.TryGetValue(obj, out var listener))
        {
            listener[eventName]?.Invoke(arg);
        }
    }

    public void AddTempEvent(string eventName, Action<object> action)
    {
        tempEventDict.TryAdd(eventName, action);
    }

    public void RemoveTempEvent(string eventName, Action<object> action)
    {
        tempEventDict.Remove(eventName);
    }

    public void PostTempEvent(string eventName, object arg)
    {
        tempEventDict[eventName]?.Invoke(arg);
        tempEventDict.Remove(eventName);
    }
}