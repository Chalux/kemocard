using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Buff;

public partial class B15 : BaseBuff
{
    private float _tempValue;
    public B15()
    {
        Id = "15";
        Name = "强化武装";
        Description = "己方/全体/3回合/物理防御提高2000+5%当前自身的生命上限";
        Duration = 3;
        Type = BuffType.BUFF;
        StackNum = 1;
        StackLimit = 1;
        Tags.Add(BuffTag.AddPDefense);
        _tempValue = Owner.MaxHealth * 0.05f;
    }

    public override void RefreshProps(ref TempPropStruct inStruct)
    {
        inStruct.AddPDefense += 2000 + _tempValue;
    }
}