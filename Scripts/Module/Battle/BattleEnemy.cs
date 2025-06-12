using System;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.Battle;

public partial class BattleEnemy : BasePawn
{
    public int CurrentHealth = 0;
    public bool IsDead = false;

    public BattleEnemy(BasePawn pawn)
    {
        CurrentHealth = MaxHealth;
    }

    public void ExecuteBuffs()
    {
        foreach (var keyValuePair in Buffs)
        {
            keyValuePair.Value?.ApplyBuff();
        }
    }

    public virtual void Action()
    {
    }
}

public class EnemyAction
{
    public int Priority;
    public Action<EnemyAction> Action;
    public BattleEnemy User;
    public BasePawn Target;
}