using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using cfg.card;
using Godot;
using kemocard.Scripts.Card;
using kemocard.Scripts.Module.Battle;
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

    private const string BannerTipPath = "res://Components/Banner/BannerTip.tscn";

    public static void ShowBannerHint(string msg)
    {
        var banner = GameCore.Root.Banner;
        var scene = ResourceLoader.Load<PackedScene>(BannerTipPath);
        if (scene == null || banner == null) return;
        var tip = scene.Instantiate<BannerTip>();
        tip.Lab.Text = msg;
        tip.Timer.Timeout += tip.QueueFree;
        banner.AddChild(tip);
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

    public static void Shuffle<T>(List<T> list)
    {
        if (list is not { Count: > 1 }) return;

        var random = new Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static Dictionary<Attribute, Dictionary<Attribute, float>> AttributeRateDict = new()
    {
        [Attribute.WATER] = new()
        {
            [Attribute.FIRE] = 2f,
        },
        [Attribute.FIRE] = new()
        {
            [Attribute.WIND] = 2f,
        },
        [Attribute.WIND] = new()
        {
            [Attribute.EARTH] = 2f,
        },
        [Attribute.EARTH] = new()
        {
            [Attribute.ELECTRON] = 2f,
        },
        [Attribute.ELECTRON] = new()
        {
            [Attribute.WATER] = 2f,
        },
        [Attribute.LIGHT] = new()
        {
            [Attribute.DARK] = 2f,
        },
        [Attribute.DARK] = new()
        {
            [Attribute.LIGHT] = 2f,
        },
        [Attribute.NONE] = new(),
    };

    /**
     * 计算属性倍率
     */
    public static void CalculateAttributeRate(Damage damage)
    {
        float attributeRate = 1f;

        #region 查找属性关系，只保留最高的倍率

        var attr = damage.Attribute;
        var targetAttribute = damage.Target.Attribute;
        if ((attr & (int)Attribute.WATER) > 0)
        {
            attributeRate =
                (from kvp in AttributeRateDict[Attribute.WATER]
                    where (targetAttribute & (int)kvp.Key) > 0
                    select kvp.Value).Prepend(attributeRate).Max();
        }

        if ((attr & (int)Attribute.FIRE) > 0)
        {
            attributeRate =
                (from kvp in AttributeRateDict[Attribute.FIRE]
                    where (targetAttribute & (int)kvp.Key) > 0
                    select kvp.Value).Prepend(attributeRate).Max();
        }

        if ((attr & (int)Attribute.WIND) > 0)
        {
            attributeRate =
                (from kvp in AttributeRateDict[Attribute.WIND]
                    where (targetAttribute & (int)kvp.Key) > 0
                    select kvp.Value).Prepend(attributeRate).Max();
        }

        if ((attr & (int)Attribute.EARTH) > 0)
        {
            attributeRate =
                (from kvp in AttributeRateDict[Attribute.EARTH]
                    where (targetAttribute & (int)kvp.Key) > 0
                    select kvp.Value).Prepend(attributeRate).Max();
        }

        if ((attr & (int)Attribute.ELECTRON) > 0)
        {
            attributeRate =
                (from kvp in AttributeRateDict[Attribute.ELECTRON]
                    where (targetAttribute & (int)kvp.Key) > 0
                    select kvp.Value).Prepend(attributeRate).Max();
        }

        if ((attr & (int)Attribute.LIGHT) > 0)
        {
            attributeRate =
                (from kvp in AttributeRateDict[Attribute.LIGHT]
                    where (targetAttribute & (int)kvp.Key) > 0
                    select kvp.Value).Prepend(attributeRate).Max();
        }

        if ((attr & (int)Attribute.DARK) > 0)
        {
            attributeRate =
                (from kvp in AttributeRateDict[Attribute.DARK]
                    where (targetAttribute & (int)kvp.Key) > 0
                    select kvp.Value).Prepend(attributeRate).Max();
        }

        #endregion

        damage.Value = (int)(damage.Value * attributeRate);
    }

    public static void CopyAllInheritedProperties<TParent, TChild>(TParent source, TChild target)
        where TChild : class
    {
        Type currentType = typeof(TParent);

        while (currentType != null && currentType != typeof(object))
        {
            PropertyInfo[] properties = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in properties)
            {
                PropertyInfo targetProp =
                    typeof(TChild).GetProperty(prop.Name, BindingFlags.Public | BindingFlags.Instance);

                if (targetProp != null && targetProp.CanWrite && prop.CanRead &&
                    targetProp.PropertyType == prop.PropertyType)
                {
                    object value = prop.GetValue(source);
                    targetProp.SetValue(target, value);
                }
            }

            currentType = currentType.BaseType;
        }
    }
}