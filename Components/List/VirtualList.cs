using System;
using System.Collections.Generic;
using Godot;

namespace kemocard.Components.List;

public partial class VirtualList : Control
{
    public List<object> Array { get; private set; }

    // public Func<Control> ItemFactory { get; set; } // 创建新项的方法
    public Action<Control, int, object> RenderHandler { get; set; }
    public Action<int, int> SelectedHandler { get; set; }
    public Func<int, bool> CanSelectHandler { get; set; }

    [Export] private PackedScene _itemScene; // 如果使用预制体
    [Export] private Container _contentContainer;
    [Export] private ScrollContainer _scrollView;
    private int _selectedIndex = -1;
    private int _offsetIndex;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            // if (_selectedIndex == value) return;
            var old = _selectedIndex;
            _selectedIndex = value;
            OnSelectedItem(old);
            UpdateVisibleItems();
        }
    }

    private void OnSelectedItem(int previousIndex)
    {
        SelectedHandler?.Invoke(_selectedIndex, previousIndex);
    }

    public void SetData(List<object> data)
    {
        Array = data;
        if (_contentContainer.GetChildCount() <= Array.Count)
        {
            for (var i = _contentContainer.GetChildCount(); i < Array.Count; i++)
            {
                var node = _itemScene.Instantiate<Control>();
                _contentContainer.AddChild(node);
                if (node is ISelectableItem selectableItem)
                {
                    selectableItem.List = this;
                    selectableItem.Index = i;
                    selectableItem.SetMouseFilter(MouseFilterEnum.Pass);
                    node.GuiInput += selectableItem.OnGuiInput;
                }
            }
        }
        else
        {
            for (var i = _contentContainer.GetChildCount() - 1; i >= Array.Count; i--)
            {
                var control = _contentContainer.GetChild<Control>(i);
                if (control is ISelectableItem selectableItem)
                {
                    selectableItem.List = null;
                    control.GuiInput -= selectableItem.OnGuiInput;
                }

                _contentContainer.RemoveChild(control);
                control.QueueFree();
            }
        }

        UpdateVisibleItems();
    }

    private void UpdateVisibleItems()
    {
        foreach (var child in _contentContainer.GetChildren())
        {
            if (Array != null && child is Control c) RenderHandler?.Invoke(c, c.GetIndex(), Array[c.GetIndex()]);
        }
    }
}