using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Buff;

public partial class B1 : BaseBuff
{
    public B1()
    {
        Id = "1";
        Name = "冷气凝集";
        Description = "自身/3回合/物理攻击提高7500";
        Duration = 3;
        Type = BuffType.BUFF;
        StackNum = 1;
        StackLimit = 1;
        Tags.Add(BuffTag.AddPAttack);
    }
    public override void RefreshProps(ref TempPropStruct inStruct)
    {
        inStruct.AddPAttack += 7500;
    }
}