using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

/// <summary>
/// Move as unidades em direção ao seu MoveTarget usando o move_speed
/// definido no JSON do mod (via UnitStatsComponent).
/// </summary>
public class MovementSystem : ISystem
{
    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            var transform = entity.GetComponent<TransformComponent>();
            var moveTarget = entity.GetComponent<MoveTargetComponent>();
            var stats = entity.GetComponent<UnitStatsComponent>();

            if (transform == null || moveTarget?.Target == null || stats == null) continue;

            var target = moveTarget.Target.Value;
            var toTarget = target - transform.Position;
            var distance = toTarget.Length();
            var step = stats.MoveSpeed * deltaTime;

            if (distance <= step)
            {
                // Chegou ao destino
                transform.Position = target;
                moveTarget.Target = null;
            }
            else
            {
                transform.Position += toTarget / distance * step;
            }
        }
    }
}
