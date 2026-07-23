using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

/// <summary>
/// Comportamento ambiente: unidades ociosas escolhem um ponto próximo aleatório
/// e caminham até ele de tempos em tempos. Unidades selecionadas pelo jogador
/// ficam paradas aguardando ordens.
/// </summary>
public class WanderSystem : ISystem
{
    private readonly Random _random = new();
    private readonly int _worldWidth;
    private readonly int _worldHeight;

    private const float WanderRadius = 140f;
    private const int Margin = 40;

    public WanderSystem(int worldWidth, int worldHeight)
    {
        _worldWidth = worldWidth;
        _worldHeight = worldHeight;
    }

    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            var wander = entity.GetComponent<WanderComponent>();
            var moveTarget = entity.GetComponent<MoveTargetComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            if (wander == null || moveTarget == null || transform == null) continue;

            // Personagem principal e unidades em combate não vagam
            if (entity.HasComponent<PlayerControlledComponent>()) continue;
            if (entity.GetComponent<CombatTargetComponent>()?.Target is { IsActive: true }) continue;

            wander.Cooldown -= deltaTime;
            if (wander.Cooldown > 0f || moveTarget.Target != null) continue;

            var offset = new Vector2(
                (float)(_random.NextDouble() * 2 - 1) * WanderRadius,
                (float)(_random.NextDouble() * 2 - 1) * WanderRadius);

            var destination = transform.Position + offset;
            destination.X = Math.Clamp(destination.X, Margin, _worldWidth - Margin);
            destination.Y = Math.Clamp(destination.Y, Margin, _worldHeight - Margin);

            moveTarget.Target = destination;
            wander.Cooldown = 1.5f + (float)_random.NextDouble() * 4f;
        }
    }
}
