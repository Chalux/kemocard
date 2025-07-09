using kemocard.Scripts.Common;

namespace kemocard.Scripts.Card.Scripts;

public partial class BaseBuffCard(string buffId) : BaseCardScript
{
    private string _buffId = buffId;

    public override void UseCard(BaseBattleCard parent)
    {
        var buff = StaticUtil.NewBuffOrNullById(_buffId, parent.User);
        if (buff != null) parent.User.AddBuff(buff);
    }
}