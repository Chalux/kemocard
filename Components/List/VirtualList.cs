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
    private int _visibleCount = 5; // 默认可见数量
    private int _offsetIndex;
    private const int PoolSize = 10;
    private Control[] _itemPool = new Control[PoolSize];
    private int _itemHeight = 64; // 假设每个项高度为64

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

    public override void _Ready()
    {
        Initialize();
    }

    private void Initialize()
    {
        // _scrollView = GetNode<ScrollContainer>("ScrollView");
        // _contentContainer = _scrollView.GetNode<VBoxContainer>("Content");

        for (int i = 0; i < PoolSize; i++)
        {
            var item = _itemScene.Instantiate<Control>();
            if (item is ISelectableItem selectableItem)
            {
                selectableItem.Index = i;
                selectableItem.List = this;
                selectableItem.Visible = false;
                if (selectableItem is Control temp)
                {
                    temp.GuiInput += selectableItem.OnGuiInput;
                }
            }

            _contentContainer.AddChild(item);
            _itemPool[i] = item;
        }

        UpdateVisibleItems();
    }

    private void OnSelectedItem(int previousIndex)
    {
        SelectedHandler?.Invoke(_selectedIndex, previousIndex);
    }

    public void SetData(List<object> data)
    {
        Array = data;
        UpdateVisibleItems();
    }

    private void UpdateVisibleItems()
    {
        int total = Array?.Count ?? 0;
        int start = _offsetIndex;
        int end = Math.Min(start + _visibleCount, total);

        for (int i = 0; i < PoolSize; i++)
        {
            var item = _itemPool[i];
            if (item is ISelectableItem selectableItem)
            {
                int index = start + i;
                if (index < end)
                {
                    item.Visible = true;
                    selectableItem.Index = index;
                    if (Array != null) RenderHandler?.Invoke(item, index, Array[index]);
                }
                else
                {
                    item.Visible = false;
                }
            }
        }
    }

    public void OnScrollValueChanged(float value)
    {
        _offsetIndex = (int)value;
        UpdateVisibleItems();
    }
}