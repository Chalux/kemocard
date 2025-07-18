﻿using cfg.character;
using cfg.pawn;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Card.Scripts;

public partial class C4 : BaseAttackCard
{
    protected override void CustomizeDamage(ref Damage damage, BaseBattleCard parent)
    {
        damage.Times = 3;
    }
}