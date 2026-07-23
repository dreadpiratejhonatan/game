using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

/// <summary>
/// Controla a animação dos sprites: alterna frames de caminhada enquanto a unidade
/// se move e vira o sprite na direção do movimento (flip horizontal).
/// </summary>
public class AnimationSystem : ISystem
{
    private const float FrameDuration = 0.14f;

    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            var sprite = entity.GetComponent<SpriteComponent>();
            if (sprite == null) continue;

            var moveTarget = entity.GetComponent<MoveTargetComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var isMoving = moveTarget?.Target != null;

            if (isMoving && transform != null)
            {
                var dx = moveTarget!.Target!.Value.X - transform.Position.X;
                if (Math.Abs(dx) > 1f)
                    sprite.FacingLeft = dx < 0;

                sprite.FrameTimer += deltaTime;
                if (sprite.FrameTimer >= FrameDuration)
                {
                    sprite.FrameTimer = 0f;
                    sprite.CurrentFrame = sprite.CurrentFrame == 1 ? 2 : 1;
                }
            }
            else
            {
                sprite.CurrentFrame = 0;
                sprite.FrameTimer = 0f;
            }
        }
    }
}
