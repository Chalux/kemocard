using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Buff;

public partial class B9 : BaseBuff
{
    public B9()
    {
        Id = "9";
        Name = "灵舞";
        Description = "3回合/物理攻击提高6000";
        Duration = 3;
        Type = BuffType.BUFF;
        StackNum = 1;
        StackLimit = 1;
        Tags.Add(BuffTag.AddPAttack);
    }

    public override void RefreshProps(ref TempPropStruct inStruct)
    {
        inStruct.AddPAttack += 6000;
    }
}