using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Estado de combate: cooldown de ataque e flash visual ao receber dano.
/// </summary>
public class CombatStateComponent : IComponent
{
    public float AttackCooldownRemaining { get; set; }
    public float HitFlashTimer { get; set; }
}
