using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Buff;

public partial class B14 : BaseBuff
{
    public B14()
    {
        Id = "14";
        Name = "魔力拒止";
        Description = "己方/全体/3回合/魔法防御提高2000,抽卡+1";
        Duration = 3;
        Type = BuffType.BUFF;
        StackNum = 1;
        StackLimit = 1;
        Tags.Add(BuffTag.AddMDefense);
        Tags.Add(BuffTag.AddDraw);
    }

    public override void RefreshProps(ref TempPropStruct inStruct)
    {
        inStruct.AddMDefense += 2000;
        inStruct.AddDraw += 1;
    }
}