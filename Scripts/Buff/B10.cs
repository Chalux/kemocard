using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Buff;

public partial class B10 : BaseBuff
{
    public B10()
    {
        Id = "10";
        Name = "天轮";
        Description = "2回合/回复量提高2000";
        Duration = 3;
        Type = BuffType.BUFF;
        StackNum = 1;
        StackLimit = 1;
        Tags.Add(BuffTag.AddHeal);
    }

    public override void RefreshProps(ref TempPropStruct inStruct)
    {
        inStruct.AddHeal += 2000;
    }
}