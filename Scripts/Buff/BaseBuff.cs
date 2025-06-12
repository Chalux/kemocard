using System.Collections.Generic;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Buff;

public partial class BaseBuff : GodotObject
{
    // Buff唯一标识
    public string Id = "";
    public string Name = "";
    public string Description = "";
    public int Duration = 0;
    public BuffType Type = BuffType.NONE;
    public int StackNum = 0;
    public int StackLimit = 0;
    public HashSet<BuffTag> Tags = new();

    public BaseCharacter Owner;
    public BaseCharacter Causer;

    public virtual void RefreshProps(ref readonly TempPropStruct inStruct)
    {
    }

    public virtual void ApplyBuff(BuffTag tag = BuffTag.None)
    {
    }

    public virtual void OnRemoved()
    {
    }

    ~BaseBuff()
    {
        GameCore.EventBus.RemoveObjAllEvents(this);
    }
}

public enum BuffType
{
    NONE,
    BUFF,
    DEBUFF
}

public enum BuffTag
{
    None,
    AddHealth,
    ExtraHealth,
    AddPAttack,
    ExtraPAttack,
    AddMAttack,
    ExtraMAttack,
    AddPDefense,
    ExtraPDefense,
    AddMDefense,
    ExtraMDefense,
    AddHeal,
    ExtraHeal,
    AddDraw,
    Attacked,
    Attack,
    TurnEnd,
    TurnStart,
    BattleStart,
}