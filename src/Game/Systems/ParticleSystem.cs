using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

/// <summary>
/// Atualiza partículas visuais: spawna poeira ao andar, move e mata por tempo de vida.
/// </summary>
public class ParticleSystem : ISystem
{
    private readonly Random _random = new();

    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            var emitter = entity.GetComponent<ParticleEmitterComponent>();
            if (emitter == null) continue;

            var moveTarget = entity.GetComponent<MoveTargetComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var isMoving = moveTarget?.Target != null && transform != null;

            // Spawna partículas de poeira ao caminhar
            if (isMoving)
            {
                emitter.SpawnTimer -= deltaTime;
                if (emitter.SpawnTimer <= 0f)
                {
                    emitter.SpawnTimer = emitter.SpawnRate;
                    SpawnDustParticle(emitter, transform!.Position);
                }
            }

            // Atualiza partículas existentes
            for (int i = emitter.Particles.Count - 1; i >= 0; i--)
            {
                var p = emitter.Particles[i];
                p.Life -= deltaTime;
                
                if (p.Life <= 0f)
                {
                    emitter.Particles.RemoveAt(i);
                    continue;
                }

                p.Position += p.Velocity * deltaTime;
                p.Velocity *= 0.95f; // fricção
                
                // Fade out baseado no tempo de vida
                var alpha = p.Life / p.MaxLife;
                p.Color = new Color(p.Color, alpha * 0.6f);
            }
        }
    }

    private void SpawnDustParticle(ParticleEmitterComponent emitter, Vector2 position)
    {
        var offsetX = (float)(_random.NextDouble() * 16 - 8);
        var offsetY = (float)(_random.NextDouble() * 8 + 8);
        
        emitter.Particles.Add(new Particle
        {
            Position = position + new Vector2(offsetX, offsetY),
            Velocity = new Vector2(
                (float)(_random.NextDouble() * 10 - 5),
                (float)(_random.NextDouble() * -5 - 5)
            ),
            Color = new Color(180, 170, 150, 200),
            Life = 0.5f + (float)_random.NextDouble() * 0.3f,
            MaxLife = 0.8f
        });
    }
}
