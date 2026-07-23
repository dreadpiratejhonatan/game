using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

/// <summary>
/// Move as unidades em direção ao MoveTarget e mantém dentro dos limites do mundo.
/// </summary>
public class MovementSystem : ISystem
{
    private readonly int _worldWidth;
    private readonly int _worldHeight;
    private const float Margin = 8f;

    public MovementSystem(int worldWidth, int worldHeight)
    {
        _worldWidth = worldWidth;
        _worldHeight = worldHeight;
    }

    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            var transform = entity.GetComponent<TransformComponent>();
            var moveTarget = entity.GetComponent<MoveTargetComponent>();
            var stats = entity.GetComponent<UnitStatsComponent>();
            var render = entity.GetComponent<RenderComponent>();

            if (transform == null || moveTarget?.Target == null || stats == null) continue;

            var target = moveTarget.Target.Value;
            var toTarget = target - transform.Position;
            var distance = toTarget.Length();
            var step = stats.MoveSpeed * deltaTime;

            if (distance <= step)
            {
                transform.Position = target;
                moveTarget.Target = null;
            }
            else
            {
                transform.Position += toTarget / distance * step;
            }

            var maxX = _worldWidth - (render?.Size.X ?? 32) - Margin;
            var maxY = _worldHeight - (render?.Size.Y ?? 32) - Margin;
            transform.Position = new Vector2(
                Math.Clamp(transform.Position.X, Margin, maxX),
                Math.Clamp(transform.Position.Y, Margin, maxY));
        }
    }
}
