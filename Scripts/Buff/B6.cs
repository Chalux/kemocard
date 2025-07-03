using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Buff;

public partial class B6 : BaseBuff
{
    public B6()
    {
        Id = "6";
        Name = "疾走型地鼠";
        Description = "己方/全体/2回合/物理防御增加3000,抽卡+1";
        Duration = 2;
        Type = BuffType.BUFF;
        StackNum = 1;
        StackLimit = 1;
        Tags.Add(BuffTag.AddPDefense);
        Tags.Add(BuffTag.AddDraw);
    }
    public override void RefreshProps(ref TempPropStruct inStruct)
    {
        inStruct.AddPDefense += 3000;
        inStruct.AddDraw += 1;
    }
}