using System;
using System.Collections.Generic;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.Enemy;
using kemocard.Scripts.Pawn;

namespace kemocard.Scripts.Module.Battle;

public sealed class BattleEnemy : BasePawn, IBattlePawn
{
    private int _currentHealth;
    private EnemyScript _script;

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
        var path = StaticUtil.GetEnemyScriptPath(Id);
        if (!FileAccess.FileExists(path)) return;
        var res = ResourceLoader.Load<CSharpScript>(path);
        _script = res?.New().As<EnemyScript>();
        _actionDesc = _script?.GetActionDesc();
    }

    public string ActionDesc = "";
    private List<string> _actionId = [];
    private Dictionary<string, string> _actionDesc = new();

    public void Action()
    {
        _script?.Action(this, _actionId);
    }

    public string GetEnemyDesc()
    {
        string result = "";
        result +=
            $"{Name}\n生命:{CurrentHealth}/{MaxHealth}\n物理攻击:{PAttack}  魔法攻击:{MAttack}\n物理防御:{PDefense}  魔法防御:{MDefense}\n行为:{ActionDesc}";
        return result;
    }

    public void UpdateActionId()
    {
        _actionId = _script?.UpdateActionId() ?? [];
    }
}

public class EnemyAction
{
    public int Priority;
    public Action<EnemyAction> Action;
    public BattleEnemy User;
    public BasePawn Target;
}