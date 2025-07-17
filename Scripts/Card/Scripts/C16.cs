using cfg.character;
using cfg.pawn;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class C16 : BaseAttackCard
{
    public override void UpdateRealTimeValue(BaseBattleCard parent, ref float result)
    {
        result += 5000 * parent.RealTimeChain;
    }
}