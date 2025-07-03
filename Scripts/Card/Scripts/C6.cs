using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class C6 : BaseCardScript
{
    public override void UseCard(BaseBattleCard parent)
    {
        var mod = GameCore.ControllerMgr.GetControllerModel<BattleModel>(ControllerType.Battle);
        if (mod == null) return;
        var buff = StaticUtil.NewBuffOrNullById("6", parent.User);
        if (buff != null)
        {
            mod.Teammates.ForEach(teammate => teammate.AddBuff(buff));
        }
    }
}