using System.Collections.Generic;
using Godot;

namespace kemocard.Components.List;

public class ItemPool<T>(PackedScene scene, Node parent)
    where T : CanvasItem, new()
{
    private readonly Queue<T> _pool = new();
    public Queue<T> Pool => _pool;

    public T Get()
    {
        if (_pool.Count > 0)
        {
            var item = _pool.Dequeue();
            item.Visible = true;
            return item;
        }

        var newItem = (T)scene.Instantiate();
        parent.AddChild(newItem);
        return newItem;
    }

    public void Return(T item)
    {
        item.Visible = false;
        _pool.Enqueue(item);
    }

    public void Clear()
    {
        foreach (var item in _pool)
        {
            item.QueueFree();
        }

        _pool.Clear();
    }
}