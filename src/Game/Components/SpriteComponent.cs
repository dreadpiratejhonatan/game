using Microsoft.Xna.Framework.Graphics;
using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Sprite animado da unidade (texturas geradas proceduralmente).
/// Frames: [0] parado, [1] e [2] alternando durante a caminhada.
/// </summary>
public class SpriteComponent : IComponent
{
    public Texture2D[] Frames { get; }
    public int CurrentFrame { get; set; }
    public float FrameTimer { get; set; }
    public bool FacingLeft { get; set; }

    public SpriteComponent(Texture2D[] frames)
    {
        Frames = frames;
    }
}
