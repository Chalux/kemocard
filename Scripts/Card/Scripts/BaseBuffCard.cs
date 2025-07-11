using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class BaseBuffCard(string buffId) : BaseCardScript
{
    private string _buffId = buffId;
    /**
     * Mod可能为null，使用时注意
     */
    protected BattleModel Mod = GameCore.ControllerMgr.GetControllerModel<BattleModel>(ControllerType.Battle);

    public override void UseCard(BaseBattleCard parent)
    {
        var buff = StaticUtil.NewBuffOrNullById(_buffId, parent.User);
        if (buff != null) parent.User.AddBuff(buff);
    }
}