using System;
using System.Collections.Generic;
using System.Linq;
using cfg.card;
using Godot;
using kemocard.Scripts.Card;
using Attribute = cfg.pawn.Attribute;

namespace kemocard.Scripts.Common;

public static class StaticUtil
{
    // public static void UpdateList(ItemList list, Array array, string itemResPath)
    // {
    //     if (list.GetChildCount() < array.Length)
    //     {
    //         var itemRes = ResourceLoader.Load<PackedScene>(itemResPath);
    //         for (int i = list.GetChildCount(); i < array.Length; i++)
    //         {
    //             var item = itemRes.Instantiate();
    //             list.AddChild(item);
    //         }
    //     }
    // }

    public static string GetAttributeName(int attribute, bool needEnd = true)
    {
        List<string> attrs = [];
        if ((attribute & (int)Attribute.WATER) > 0)
        {
            attrs.Add("水");
        }

        if ((attribute & (int)Attribute.FIRE) > 0)
        {
            attrs.Add("火");
        }

        if ((attribute & (int)Attribute.WIND) > 0)
        {
            attrs.Add("风");
        }

        if ((attribute & (int)Attribute.EARTH) > 0)
        {
            attrs.Add("土");
        }

        if ((attribute & (int)Attribute.ELECTRON) > 0)
        {
            attrs.Add("电");
        }

        if ((attribute & (int)Attribute.LIGHT) > 0)
        {
            attrs.Add("光");
        }

        if ((attribute & (int)Attribute.DARK) > 0)
        {
            attrs.Add("暗");
        }

        return String.Join("/", attrs) + (needEnd ? "属性" : "");
    }

    public static void ShowHint(string msg)
    {
        var panel = GameCore.Root.HintPanel;
        panel.SetVisible(true);
        var label = GameCore.Root.HintLabel;
        label.Text = msg;
        var newSize = new Vector2(label.GetContentWidth(), label.GetContentHeight());
        label.SetCustomMinimumSize(newSize);
        label.SetSize(newSize);
        panel.SetCustomMinimumSize(newSize);
        panel.SetSize(newSize);
    }

    public static void HideHint()
    {
        GameCore.Root.HintPanel.SetVisible(false);
    }

    public static string GetTagName(List<Tag> tags)
    {
        return string.Join("/", tags.Select(GetSingleTagName));
    }

    public static string GetSingleTagName(Tag tag)
    {
        return tag switch
        {
            Tag.PATTACK => "物理攻击",
            Tag.MATTACK => "魔法攻击",
            Tag.GUARD => "防御",
            Tag.HEAL => "治疗",
            Tag.SUPPORT => "支援",
            Tag.PDEFENSE => "物理防御",
            Tag.MDEFENSE => "魔法防御",
            Tag.DEBLOCK => "解封",
            Tag.PBLOCK => "物理弱化",
            Tag.MBLOCK => "魔法弱化",
            Tag.WATER => "水属性",
            Tag.FIRE => "火属性",
            Tag.WIND => "风属性",
            Tag.EARTH => "土属性",
            Tag.ELECTRON => "电属性",
            Tag.LIGHT => "光属性",
            Tag.DARK => "暗属性",
            _ => ""
        };
    }

    public static string GetCardDesc(BaseCard card)
    {
        return GetCardDesc(card.Id);
    }

    public static string GetCardDesc(string cardId)
    {
        var card = GameCore.Tables.TbCard.GetOrDefault(cardId);
        if (card == null) return "";
        return $"{card.Name}\t{GetAttributeName((int)card.Attribute)}\tCost{card.Cost}\t{card.Desc}";
    }
}