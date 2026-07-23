using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Alvo de combate atual. Quando definido, a unidade persegue e ataca.
/// </summary>
public class CombatTargetComponent : IComponent
{
    public Entity? Target { get; set; }
}
