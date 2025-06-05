using System.Collections.Generic;
using cfg.card;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Card;

public partial class BaseCard : GodotObject
{
    public string Id { get; protected set; }
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

    public BaseCard(string configId)
    {
        var conf = GameCore.Tables.TbCard.Get(configId);
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
    }

    public virtual void UseCard(BaseCharacter owner, BattleContext context)
    {
    }
}