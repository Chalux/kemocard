using Godot;
using kemocard.Scripts.Buff;
using kemocard.Scripts.Common;

namespace kemocard.Scripts.Card.Scripts;

public partial class C1 : BaseCardScript
{
    public override void UseCard(BaseBattleCard parent)
    {
        var buff = StaticUtil.NewBuffOrNullById("1", parent.User);
        if (buff != null) parent.User.AddBuff(buff);
    }
}