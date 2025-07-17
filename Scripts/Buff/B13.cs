using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Buff;

public partial class B13 : BaseBuff
{
    public B13()
    {
        Id = "13";
        Name = "对阵战线";
        Description = "自身/3回合/嘲讽:2，生命上限提高15000";
        Duration = 3;
        Type = BuffType.BUFF;
        StackNum = 1;
        StackLimit = 1;
        IsUnique = false;
        Tags.Add(BuffTag.Taunt);
        Tags.Add(BuffTag.AddHealth);
    }
    
    public override void RefreshProps(ref TempPropStruct inStruct)
    {
        inStruct.AddHealth += 15000;
        inStruct.Taunt += 2;
    }
}