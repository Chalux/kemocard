using cfg.character;
using cfg.pawn;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class C9 : BaseCardScript
{
    public override void UseCard(BaseBattleCard parent)
    {
        var buff = StaticUtil.NewBuffOrNullById("9", parent.User);
        if (buff != null) parent.User.AddBuff(buff);
    }
}