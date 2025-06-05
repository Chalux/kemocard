using System;
using System.Collections.Generic;
using cfg.card;
using Godot;
using kemocard.Scripts.Common;

namespace kemocard.Components.TagSelector;

public partial class TagSelector : ItemList
{
    public HashSet<Tag> CurrentTags { get; private set; }
    public Action OnTagSelected;

    public override void _Ready()
    {
        base._Ready();
        Clear();
        CurrentTags = [];
        var values = Enum.GetValues<Tag>();
        foreach (var tag in values)
        {
            AddItem(StaticUtil.GetSingleTagName(tag));
        }

        MultiSelected += OnItemSelected;
    }

    private void OnItemSelected(long index, bool selected)
    {
        CurrentTags.Clear();
        var values = Enum.GetValues<Tag>();
        foreach (var idx in GetSelectedItems())
        {
            CurrentTags.Add(values[idx]);
        }

        OnTagSelected?.Invoke();
    }
}