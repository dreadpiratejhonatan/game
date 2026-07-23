using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Componente que define como uma entidade deve ser renderizada (placeholder geométrico).
/// </summary>
public class RenderComponent : IComponent
{
    public Vector2 Size { get; set; }
    public Color Color { get; set; }
    public RenderShape Shape { get; set; }
    
    public RenderComponent(Vector2 size, Color color, RenderShape shape = RenderShape.Rectangle)
    {
        Size = size;
        Color = color;
        Shape = shape;
    }
}

public enum RenderShape
{
    Rectangle,
    Circle
}
