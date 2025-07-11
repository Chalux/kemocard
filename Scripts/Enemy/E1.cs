using System;
using System.Collections.Generic;
using kemocard.Scripts.Common;

namespace kemocard.Scripts.Enemy;

public partial class E1 : EnemyScript
{
    public override Dictionary<string, string> GetActionDesc()
    {
        return new Dictionary<string, string>
        {
            {
                "all_attack", "全体攻击，5000+(0.5*物理攻击)点物理伤害"
            },
            { "self_pdefense", "自身/3回合/物理防御+5000" },
            { "self_pattack", "自身/永久/物理攻击+2000,可叠加" },
            { "single_attack", "随机单体攻击，7500+(物理攻击)点物理伤害" }
        };
    }

    public override List<string> UpdateActionId()
    {
        List<string> actionIds = ["all_attack", "self_pdefense", "single_attack", "self_pattack"];
        var turn = Mod?.TurnCount ?? 0;
        var r = new Random();
        // 随机选两个，排除加攻
        if (turn > 4) return [actionIds[r.Next(2)], actionIds[r.Next(2)]];
        return turn switch
        {
            1 => ["self_pattack", "all_attack"],
            2 => ["self_pdefense", "single_attack"],
            3 => ["self_pattack", "single_attack"],
            4 => ["all_attack", "all_attack"],
            _ => []
        };
    }
}