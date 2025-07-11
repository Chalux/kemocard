using System.Collections.Generic;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Battle;
using kemocard.Scripts.MVC;

namespace kemocard.Scripts.Enemy;

public partial class EnemyScript : GodotObject
{
    /**
     * Mod可能为null,使用时注意
     */
    protected BattleModel Mod = GameCore.ControllerMgr.GetControllerModel<BattleModel>(ControllerType.Battle);

    public virtual void Action(BattleEnemy parent, List<string> actionId)
    {
    }

    public virtual Dictionary<string, string> GetActionDesc()
    {
        return [];
    }

    public virtual List<string> UpdateActionId()
    {
        return [];
    }
}