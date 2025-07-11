using System.Collections.Generic;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;
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
    public bool IsUnique = true;
    public string HashCode = "";
    public readonly HashSet<BuffTag> Tags = [];
    /**
     * Mod可能为null,使用时注意
     */
    protected BattleModel Mod = GameCore.ControllerMgr.GetControllerModel<BattleModel>(ControllerType.Battle);

    public BasePawn Owner;
    public BasePawn Causer;

    public virtual void RefreshProps(ref TempPropStruct inStruct)
    {
    }

    public virtual void ApplyBuff(ref object data, BuffTag tag = BuffTag.None)
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

    /** 增加最大HP 加区 */
    AddHealth,

    /** 增加最大HP 乘区 */
    ExtraHealth,
    AddPAttack,
    ExtraPAttack,
    AddMAttack,
    ExtraMAttack,
    AddPDefense,
    ExtraPDefense,
    AddMDefense,
    ExtraMDefense,

    /** 增加回复量 */
    AddHeal,

    /** 增加回复量 乘区 */
    ExtraHeal,
    AddDraw,

    /** 伤害结算前 */
    BeforeAttacked,

    /** 伤害结算后 */
    Attacked,
    Attack,
    TurnEnd,
    TurnStart,
    BattleStart,

    /** 释放回复效果 */
    Heal,

    /** 被治疗 */
    Healed,
}