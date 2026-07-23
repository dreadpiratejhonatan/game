using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Componente que representa a posição, rotação e escala de uma entidade no mundo.
/// </summary>
public class TransformComponent : IComponent
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; } = Vector2.One;
    
    public TransformComponent(Vector2 position)
    {
        Position = position;
    }
    
    public TransformComponent(float x, float y) : this(new Vector2(x, y))
    {
    }
}
