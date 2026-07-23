using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.Core;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

public class WanderSystem : ISystem
{
    private readonly Random _random = new();
    private readonly WorldBounds _bounds;

    private const float WanderRadius = 140f;
    private const int Margin = 40;

    public WanderSystem(WorldBounds bounds)
    {
        _bounds = bounds;
    }

    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            var wander = entity.GetComponent<WanderComponent>();
            var moveTarget = entity.GetComponent<MoveTargetComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            if (wander == null || moveTarget == null || transform == null) continue;

            if (entity.HasComponent<PlayerControlledComponent>()) continue;
            if (entity.GetComponent<CombatTargetComponent>()?.Target is { IsActive: true }) continue;

            wander.Cooldown -= deltaTime;
            if (wander.Cooldown > 0f || moveTarget.Target != null) continue;

            var offset = new Vector2(
                (float)(_random.NextDouble() * 2 - 1) * WanderRadius,
                (float)(_random.NextDouble() * 2 - 1) * WanderRadius);

            var destination = transform.Position + offset;
            destination.X = Math.Clamp(destination.X, Margin, _bounds.Width - Margin);
            destination.Y = Math.Clamp(destination.Y, Margin, _bounds.Height - Margin);

            moveTarget.Target = destination;
            wander.Cooldown = 1.5f + (float)_random.NextDouble() * 4f;
        }
    }
}
