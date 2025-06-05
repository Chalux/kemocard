using Godot;
using kemocard.Scripts.Common;

namespace kemocard.Scripts.Pawn;

public partial class BasePawn : GodotObject
{
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
    protected readonly Godot.Collections.Dictionary<string, Buff.BaseBuff> Buffs = [];

    public virtual void InitFromConfig(string configId)
    {
        var conf = GameCore.Tables.TbPawnBaseProp.Get(configId);
        if (conf == null) return;
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
        RefreshProps();
    }

    public void AddBuff(Buff.BaseBuff newBaseBuff)
    {
        if (newBaseBuff == null) return;
        if (Buffs.TryGetValue(newBaseBuff.Id, out var buff))
        {
            if (buff.StackLimit > 0 && buff.StackNum + 1 > buff.StackLimit)
            {
                return;
            }

            buff.StackNum++;
        }
        else
        {
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