using System.Collections.Generic;
using cfg.card;
using Godot;
using kemocard.Components.List;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;

namespace kemocard.Components.Card;

public partial class CardItem : Control, ISelectableItem
{
    [Export] private Control _cardControl;
    [Export] private TextureRect _selectedImg;
    [Export] private Label _costLab;
    [Export] private Label _descLab;
    [Export] private Label _attributeLab;
    [Export] private TextureRect _icon;
    public BaseCard Card { get; private set; }

    public void Init(BaseCard card)
    {
        Card = card;
        if (FileAccess.FileExists(card.Icon)) _icon.Texture = ResourceLoader.Load<CompressedTexture2D>(card.Icon);
        SetCost(Card.Cost);
        SetDescByTag(Card.Tags);
        SetAttribute(Card.Attribute);
    }

    public void SetCost(int cost)
    {
        _costLab.Text = cost.ToString();
    }

    public void SetDescByTag(HashSet<Tag> tags)
    {
        if (tags.Contains(Tag.SUPPORT))
        {
            _descLab.Text = "支";
        }
        else if (tags.Contains(Tag.PDEFENSE) || tags.Contains(Tag.MDEFENSE))
        {
            _descLab.Text = "防";
        }
        else if (tags.Contains(Tag.PBLOCK) || tags.Contains(Tag.MBLOCK))
        {
            _descLab.Text = "弱";
        }
        else if (tags.Contains(Tag.DEBLOCK))
        {
            _descLab.Text = "解";
        }
        else if (tags.Contains(Tag.PATTACK))
        {
            _descLab.Text = "物";
        }
        else if (tags.Contains(Tag.MATTACK))
        {
            _descLab.Text = "魔";
        }
        else if (tags.Contains(Tag.HEAL))
        {
            _descLab.Text = "治";
        }
        else
        {
            _descLab.Text = "支";
        }
    }

    public void SetAttribute(int attribute)
    {
        _attributeLab.Text = StaticUtil.GetAttributeName(attribute, false);
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            _selectedImg.SetVisible(true);
            _cardControl.Modulate = Colors.DarkGray;
        }
        else
        {
            _selectedImg.SetVisible(false);
            _cardControl.Modulate = Colors.White;
        }
    }

    public void Clear()
    {
        _attributeLab.Text = "";
        _costLab.Text = "";
        _descLab.Text = "";
        _selectedImg.SetVisible(false);
        _cardControl.Modulate = Colors.White;
        _icon.Texture = null;
        Card = null;
    }

    public int Index { get; set; }
    public VirtualList List { get; set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void OnMouseExited()
    {
        StaticUtil.HideHint();
    }

    private void OnMouseEntered()
    {
        if (Card != null) StaticUtil.ShowHint(Card.Description);
        else StaticUtil.HideHint();
    }

    public override void _ExitTree()
    {
        MouseEntered -= OnMouseEntered;
        MouseExited -= OnMouseExited;
        base._ExitTree();
    }
}