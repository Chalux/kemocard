using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Buff;

public partial class B7 : BaseBuff
{
    public B7()
    {
        Id = "7";
        Name = "疾走型地鼠";
        Description = "敌方/单体/2回合/物理攻击降低2000,物理防御降低2000";
        Duration = 2;
        Type = BuffType.DEBUFF;
        StackNum = 1;
        StackLimit = 1;
        Tags.Add(BuffTag.AddPAttack);
        Tags.Add(BuffTag.AddPDefense);
    }

    public override void RefreshProps(ref TempPropStruct inStruct)
    {
        inStruct.AddPDefense -= 2000;
        inStruct.AddPAttack -= 2000;
    }
}