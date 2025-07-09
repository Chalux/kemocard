using System.Collections.Generic;
using System.Linq;
using cfg.character;
using Godot;
using Godot.Collections;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using Newtonsoft.Json;
using Attribute = cfg.pawn.Attribute;

namespace kemocard.Scripts.Pawn;

public class BaseCharacter : BasePawn
{
    public float BaseHeal;
    public Role Role = Role.NORMAL;
    [JsonProperty] public List<BaseCard> Cards = [];

    public sealed override bool InitFromConfig(string configId, bool fromSave = false)
    {
        var conf = GameCore.Tables.TbHeroBaseProp.GetOrDefault(configId);
        if (conf == null) return false;
        Id = configId;
        Attribute = (int)conf.Attribute;
        Role = conf.Role;
        Name = conf.Name;
        Icon = conf.Icon;
        Description = conf.Description;
        ImagePath = conf.ImagePath;
        Race = conf.Race;
        if (!fromSave)
        {
            foreach (var i in conf.CardList)
            {
                AddCard(i);
            }
        }

        RefreshCardProps();
        RefreshProps();
        return true;
    }

    public int Heal;
    public int Draw;

    public override void RefreshProps()
    {
        var oldMaxHealth = MaxHealth;
        var oldPAttack = PAttack;
        var oldMAttack = MAttack;
        var oldPDefense = PDefense;
        var oldMDefense = MDefense;
        var oldHeal = Heal;
        var oldDraw = Draw;
        TempPropStruct tempProp = new();
        foreach (var buff in Buffs)
        {
            buff.Value.RefreshProps(ref tempProp);
        }

        MaxHealth = (int)((BaseHealth + tempProp.AddHealth + CardHealth) * (1 + tempProp.ExtraHealth));
        PAttack = (int)((BasePAttack + tempProp.AddPAttack + CardPAttack) * (1 + tempProp.ExtraPAttack));
        MAttack = (int)((BaseMAttack + tempProp.AddMAttack + CardMAttack) * (1 + tempProp.ExtraMAttack));
        PDefense = (int)((BasePDefense + tempProp.AddPDefense + CardPDefense) * (1 + tempProp.ExtraPDefense));
        MDefense = (int)((BaseMDefense + tempProp.AddMDefense + CardMDefense) * (1 + tempProp.ExtraMDefense));
        Heal = (int)((BaseHeal + tempProp.AddHealth + CardHeal) * (1 + tempProp.ExtraHealth));
        Draw = 1 + tempProp.AddDraw;

        if (oldMaxHealth != MaxHealth || oldPAttack != PAttack || oldMAttack != MAttack || oldPDefense != PDefense ||
            oldMDefense != MDefense || oldHeal != Heal || oldDraw != Draw)
        {
            GameCore.EventBus.PostEvent(CommonEvent.PlayerPropUpdate, this);
        }
    }

    public void AddCard(BaseCard newCard)
    {
        Cards.Add(newCard);
        GameCore.EventBus.PostEvent(CommonEvent.PlayerDeckUpdate, this);
        RefreshCardProps();
    }

    public void AddCard(string configId)
    {
        BaseCard card = new(configId);
        AddCard(card);
    }

    public void RemoveCard(int index)
    {
        Cards.RemoveAt(index);
        GameCore.EventBus.PostEvent(CommonEvent.PlayerDeckUpdate, this);
        RefreshCardProps();
    }

    public void RemoveCard(BaseCard card)
    {
        if (Cards.Remove(card))
        {
            GameCore.EventBus.PostEvent(CommonEvent.PlayerDeckUpdate, this);
            RefreshCardProps();
        }
    }

    public void RemoveCardById(string id)
    {
        for (var i = 0; i < Cards.Count; i++)
        {
            if (Cards[i].Id == id)
            {
                RemoveCard(i);
                break;
            }
        }
    }

    public List<BaseCard> GetDeck()
    {
        return Cards;
    }

    public int CardHealth { get; protected set; }
    public int CardPDefense { get; protected set; }
    public int CardMDefense { get; protected set; }
    public int CardPAttack { get; protected set; }
    public int CardMAttack { get; protected set; }
    public int CardHeal { get; protected set; }

    public void RefreshCardProps()
    {
        CardHealth = CardPDefense = CardPAttack = CardMDefense = CardMAttack = 0;
        foreach (var card in Cards)
        {
            CardHealth += card.Health;
            CardPDefense += card.PDefense;
            CardPAttack += card.PAttack;
            CardMDefense += card.MDefense;
            CardMAttack += card.MAttack;
            CardHeal += card.Heal;
        }

        RefreshProps();
    }

    public string GetDeckDesc()
    {
        return $"{StaticUtil.GetAttributeName(Attribute)} {GetDeckString()}";
    }

    public string GetDeckString()
    {
        Godot.Collections.Dictionary<Attribute, int> attrCount = new()
        {
            { cfg.pawn.Attribute.WATER, 0 },
            { cfg.pawn.Attribute.FIRE, 0 },
            { cfg.pawn.Attribute.WIND, 0 },
            { cfg.pawn.Attribute.EARTH, 0 },
            { cfg.pawn.Attribute.ELECTRON, 0 },
            { cfg.pawn.Attribute.LIGHT, 0 },
            { cfg.pawn.Attribute.DARK, 0 },
            { cfg.pawn.Attribute.NONE, 0 }
        };
        foreach (var card in Cards)
        {
            if ((card.Attribute & (int)cfg.pawn.Attribute.WATER) > 0)
            {
                attrCount[cfg.pawn.Attribute.WATER]++;
            }

            if ((card.Attribute & (int)cfg.pawn.Attribute.FIRE) > 0)
            {
                attrCount[cfg.pawn.Attribute.FIRE]++;
            }

            if ((card.Attribute & (int)cfg.pawn.Attribute.WIND) > 0)
            {
                attrCount[cfg.pawn.Attribute.WIND]++;
            }

            if ((card.Attribute & (int)cfg.pawn.Attribute.EARTH) > 0)
            {
                attrCount[cfg.pawn.Attribute.EARTH]++;
            }

            if ((card.Attribute & (int)cfg.pawn.Attribute.ELECTRON) > 0)
            {
                attrCount[cfg.pawn.Attribute.ELECTRON]++;
            }

            if ((card.Attribute & (int)cfg.pawn.Attribute.LIGHT) > 0)
            {
                attrCount[cfg.pawn.Attribute.LIGHT]++;
            }

            if ((card.Attribute & (int)cfg.pawn.Attribute.DARK) > 0)
            {
                attrCount[cfg.pawn.Attribute.DARK]++;
            }

            if ((card.Attribute & (int)cfg.pawn.Attribute.NONE) > 0)
            {
                attrCount[cfg.pawn.Attribute.NONE]++;
            }
        }

        var orderedEnumerable = attrCount.OrderByDescending(kvp => kvp.Value);
        Array<string> strList = [];
        foreach (var kvp in orderedEnumerable)
        {
            if (kvp.Value > 0)
            {
                strList.Add($"{StaticUtil.GetAttributeName((int)kvp.Key)}{kvp.Value}张");
            }
        }

        return string.Join("·", strList);
    }

    public string ToDict()
    {
        var arr = new Array<string>();
        foreach (var baseCard in Cards)
        {
            arr.Add(baseCard.Id);
        }

        return Json.Stringify(new Godot.Collections.Dictionary<string, Variant>
        {
            { "Id", Id },
            { "Cards", arr }
        });
    }

    public void FromDict(string dict)
    {
        var dic = Json.ParseString(dict).As<Godot.Collections.Dictionary<string, Variant>>();
        InitFromConfig(dic.GetValueOrDefault("Id", Variant.From("0")).AsString());
        Cards.Clear();
        var arr = dic.TryGetValue("Cards", out var value) ? value.As<Array<string>>() : [];
        foreach (var s in arr)
        {
            AddCard(s);
        }
    }

    public string GetDetailDesc()
    {
        string result = "";
        result += $"{Name}\n{StaticUtil.GetAttributeName(Attribute)}\n{Description}\n{GetAllCardDesc()}";
        return result;
    }

    public static string GetDetailDescFromConfig(string id)
    {
        string result = "";
        var config = GameCore.Tables.TbHeroBaseProp.GetOrDefault(id);
        if (config != null)
        {
            result +=
                $"{config.Name}\n{StaticUtil.GetAttributeName((int)config.Attribute)}\n{config.Description}\n{GetAllCardDescFromConfig}";
        }

        return result;
    }

    public string GetAllCardDesc()
    {
        string result = "";
        foreach (var baseCard in Cards)
        {
            result += $"{StaticUtil.GetCardDesc(baseCard)}\n";
        }

        return result;
    }

    public static string GetAllCardDescFromConfig(string id)
    {
        string result = "";
        var config = GameCore.Tables.TbHeroBaseProp.GetOrDefault(id);
        if (config != null)
        {
            foreach (var baseCard in config.CardList)
            {
                result += $"{StaticUtil.GetCardDesc(baseCard)}\n";
            }
        }

        return result;
    }
}