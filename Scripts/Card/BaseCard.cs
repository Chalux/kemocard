using System.Collections.Generic;
using System.Runtime.Serialization;
using cfg.card;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.Pawn;
using Newtonsoft.Json;

namespace kemocard.Scripts.Card;

[JsonObject(MemberSerialization.OptIn)]
public partial class BaseCard : GodotObject
{
    [JsonProperty] public string Id { get; protected set; }
    public int Health { get; protected set; }
    public int PAttack { get; protected set; }
    public int PDefense { get; protected set; }
    public int MAttack { get; protected set; }
    public int MDefense { get; protected set; }
    public int Heal { get; protected set; }
    public HashSet<Tag> Tags { get; protected set; }
    public int Cost { get; protected set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public int Attribute { get; protected set; }
    public string Icon { get; protected set; }
    public int Value { get; protected set; }
    public int Sort { get; protected set; }
    public TargetType TargetType { get; protected set; }

    public BaseCard(string configId)
    {
        if (configId != null) InitFromConfig(configId);
    }

    [OnDeserialized]
    internal void InitAfterDeserialized(StreamingContext context)
    {
        InitFromConfig(Id);
    }

    public void InitFromConfig(string configId)
    {
        var conf = GameCore.Tables.TbCard.GetOrDefault(configId);
        if (conf == null)
        {
            GD.PrintErr("卡牌配置不存在");
            conf = GameCore.Tables.TbCard.Get("0");
        }

        Id = conf.Id;
        Health = conf.Health;
        PAttack = conf.Pattack;
        PDefense = conf.Pdefense;
        MAttack = conf.Mattack;
        MDefense = conf.Mdefense;
        Heal = conf.Heal;
        Tags = conf.Tag;
        Cost = conf.Cost;
        Name = conf.Name;
        Description = conf.Desc;
        Attribute = (int)conf.Attribute;
        Icon = conf.Icon;
        Value = conf.Value;
        Sort = conf.Sort;
        TargetType = conf.Targettype;
    }

    public virtual void UseCard(BaseCharacter owner, BattleContext context)
    {
    }
}