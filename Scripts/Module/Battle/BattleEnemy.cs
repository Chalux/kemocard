using System;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.Battle;

public sealed class BattleEnemy : BasePawn, IBattlePawn
{
    private int _currentHealth;

    int IBattlePawn.MaxHealth
    {
        get => MaxHealth;
        set => MaxHealth = value;
    }

    public int CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = Math.Max(0, value);
    }

    public bool IsDead { get; set; }

    int IBattlePawn.PDefense
    {
        get => PDefense;
        set => PDefense = value;
    }

    int IBattlePawn.MDefense
    {
        get => MDefense;
        set => MDefense = value;
    }

    public BattleEnemy(BasePawn pawn)
    {
        InitFromConfig(pawn.Id);
        CurrentHealth = MaxHealth;
        Position = pawn.Position;
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