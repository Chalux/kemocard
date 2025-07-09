using System.Runtime.Serialization;
using cfg.pawn;
using Godot.Collections;
using kemocard.Scripts.Buff;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using Newtonsoft.Json;
using Vector2 = Godot.Vector2;

namespace kemocard.Scripts.Pawn;

[JsonObject(MemberSerialization.OptIn)]
public class BasePawn
{
    [JsonProperty] public string Id = "";
    protected float BaseHealth;
    protected float BasePDefense;
    protected float BaseMDefense;
    protected float BasePAttack;
    protected float BaseMAttack;
    public int Attribute = (int)cfg.pawn.Attribute.NONE;
    public string Name = "";
    public string Icon = "";
    public string Description = "";
    public string ImagePath = "";
    public Race Race = Race.UNKNOWN;

    [JsonProperty] protected readonly Dictionary<string, BaseBuff> Buffs = [];

    // 只是给怪物用的
    public Vector2 Position;

    [OnDeserialized]
    internal void InitAfterDeserialized(StreamingContext context)
    {
        InitFromConfig(Id, true);
    }

    public virtual bool InitFromConfig(string configId, bool fromSave = false)
    {
        var conf = GameCore.Tables.TbPawnBaseProp.GetOrDefault(configId);
        if (conf == null) return false;
        BaseHealth = conf.BaseHealth;
        BasePDefense = conf.BasePDefense;
        BaseMDefense = conf.BaseMDefense;
        BasePAttack = conf.BasePAttack;
        BaseMAttack = conf.BaseMAttack;
        Attribute = (int)conf.Attribute;
        Name = conf.Name;
        Icon = conf.Icon;
        Description = conf.Description;
        ImagePath = conf.ImagePath;
        Id = conf.Id;
        Race = conf.Race;
        RefreshProps();
        return true;
    }

    public void AddBuff(BaseBuff newBaseBuff)
    {
        if (newBaseBuff == null) return;
        if (Buffs.TryGetValue(newBaseBuff.Id, out var buff))
        {
            if (buff.IsUnique && newBaseBuff.IsUnique)
            {
                if (buff.HashCode == newBaseBuff.HashCode)
                {
                    newBaseBuff.HashCode = StaticUtil.GetRandomHashCode();
                }

                newBaseBuff.Owner = this;
                // 如果有独特的重复buff，则用它的hashcode作为字典的键
                Buffs.Add(newBaseBuff.HashCode, buff);

                return;
            }

            if (buff.StackLimit > 0 && buff.StackNum + 1 > buff.StackLimit)
            {
                return;
            }

            buff.StackNum++;
        }
        else
        {
            newBaseBuff.Owner = this;
            Buffs.Add(newBaseBuff.Id, newBaseBuff);
        }

        RefreshProps();
    }

    public int MaxHealth;
    public int PDefense;
    public int MDefense;
    public int PAttack;
    public int MAttack;

    public virtual void RefreshProps()
    {
        TempPropStruct tempProp = new();
        foreach (var buff in Buffs)
        {
            buff.Value.RefreshProps(inStruct: ref tempProp);
        }

        MaxHealth = (int)((BaseHealth + tempProp.AddHealth) * (1 + tempProp.ExtraHealth));
        PAttack = (int)((BasePAttack + tempProp.AddPAttack) * (1 + tempProp.ExtraPAttack));
        MAttack = (int)((BaseMAttack + tempProp.AddMAttack) * (1 + tempProp.ExtraMAttack));
        PDefense = (int)((BasePDefense + tempProp.AddPDefense) * (1 + tempProp.ExtraPDefense));
        MDefense = (int)((BaseMDefense + tempProp.AddMDefense) * (1 + tempProp.ExtraMDefense));
    }

    public virtual void OnAttacked(ref Damage damage)
    {
        object data = damage;
        foreach (var keyValuePair in Buffs)
        {
            if (keyValuePair.Value.Tags.Contains(BuffTag.Attacked))
            {
                keyValuePair.Value.ApplyBuff(ref data, BuffTag.Attacked);
            }
        }
    }

    public virtual void OnAttack(ref Damage damage)
    {
        object data = damage;
        foreach (var keyValuePair in Buffs)
        {
            if (keyValuePair.Value.Tags.Contains(BuffTag.Attack))
            {
                keyValuePair.Value.ApplyBuff(ref data, BuffTag.Attack);
            }
        }
    }

    public virtual void OnHeal(ref HealStruct heal)
    {
        object data = heal;
        foreach (var buff in Buffs)
        {
            if (buff.Value.Tags.Contains(BuffTag.Heal))
            {
                buff.Value.ApplyBuff(ref data, BuffTag.Heal);
            }
        }
    }

    public virtual void OnHealed(HealStruct heal)
    {
        object data = heal;
        foreach (var buff in Buffs)
        {
            if (buff.Value.Tags.Contains(BuffTag.Heal))
            {
                buff.Value.ApplyBuff(ref data, BuffTag.Healed);
            }
        }
    }

    public virtual void ExecuteBuffs(ref object data, BuffTag tag = BuffTag.None)
    {
        foreach (var keyValuePair in Buffs)
        {
            keyValuePair.Value?.ApplyBuff(ref data, tag);
        }
    }

    public virtual void RemoveBuff(string buffId, bool removeAll = false, int removeStack = 1)
    {
        if (!Buffs.TryGetValue(buffId, out var buff)) return;
        if (removeAll)
        {
            buff.OnRemoved();
            Buffs.Remove(buffId);
        }
        else
        {
            buff.StackNum -= removeStack;
            if (buff.StackNum <= 0)
            {
                buff.OnRemoved();
                Buffs.Remove(buffId);
            }
        }
    }
}

public struct TempPropStruct
{
    public float AddHealth = 0;
    public float ExtraHealth = 0;
    public float AddPAttack = 0;
    public float ExtraPAttack = 0;
    public float AddMAttack = 0;
    public float ExtraMAttack = 0;
    public float AddPDefense = 0;
    public float ExtraPDefense = 0;
    public float AddMDefense = 0;
    public float ExtraMDefense = 0;
    public float AddHeal = 0;
    public float ExtraHeal = 0;
    public int AddDraw = 0;

    public TempPropStruct()
    {
    }
}