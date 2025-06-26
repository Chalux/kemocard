using System;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.Battle;

public sealed class BattleEnemy : BasePawn
{
    public int CurrentHealth;
    public bool IsDead;

    public BattleEnemy(BasePawn pawn)
    {
        InitFromConfig(pawn.Id);
        CurrentHealth = MaxHealth;
        Position = pawn.Position;
    }

    public void ExecuteBuffs()
    {
        foreach (var keyValuePair in Buffs)
        {
            keyValuePair.Value?.ApplyBuff();
        }
    }

    public void Action()
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