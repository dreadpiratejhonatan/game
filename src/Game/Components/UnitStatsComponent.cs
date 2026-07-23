using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Atributos de uma unidade carregados do ModManager (data-driven).
/// </summary>
public class UnitStatsComponent : IComponent
{
    public string UnitType { get; set; } = string.Empty;
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public float MoveSpeed { get; set; }
    public float AttackDamage { get; set; }
    public float AttackRange { get; set; }
    public float AttackCooldown { get; set; } = 0.8f;
    public float DetectionRange { get; set; } = 220f;

    public UnitStatsComponent(
        string unitType,
        float maxHealth,
        float moveSpeed,
        float attackDamage,
        float attackRange,
        float attackCooldown = 0.8f,
        float detectionRange = 220f)
    {
        UnitType = unitType;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        MoveSpeed = moveSpeed;
        AttackDamage = attackDamage;
        AttackRange = attackRange;
        AttackCooldown = attackCooldown;
        DetectionRange = detectionRange;
    }
}
