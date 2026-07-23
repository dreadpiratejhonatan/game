using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Emissor de partículas visuais (poeira ao caminhar, etc).
/// </summary>
public class ParticleEmitterComponent : IComponent
{
    public List<Particle> Particles { get; } = new();
    public float SpawnTimer { get; set; }
    public float SpawnRate { get; set; } = 0.15f; // segundos entre partículas
}

public class Particle
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Color Color { get; set; }
    public float Life { get; set; }
    public float MaxLife { get; set; }
}
