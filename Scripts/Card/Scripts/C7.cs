using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class C7 : BaseCardScript
{
    public override void UseCard(BaseBattleCard parent)
    {
        var buff = StaticUtil.NewBuffOrNullById("7", parent.User);
        if (buff != null && parent.Target is { Count: > 0 })
        {
            parent.Target[0].AddBuff(buff);
        }
    }
}