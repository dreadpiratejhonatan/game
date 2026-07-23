using Microsoft.Xna.Framework;

namespace ModularGameEngine.Engine.Core;

/// <summary>
/// Câmera 2D ortográfica. Posição = canto superior esquerdo da view no espaço do mundo.
/// </summary>
public class Camera2D
{
    public Vector2 Position { get; set; }
    public int ViewportWidth { get; set; }
    public int ViewportHeight { get; set; }
    public int WorldWidth { get; set; }
    public int WorldHeight { get; set; }

    public Matrix GetViewMatrix() =>
        Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0f));

    public Vector2 ScreenToWorld(Vector2 screen) => screen + Position;

    public Vector2 ScreenToWorld(Point screen) => ScreenToWorld(screen.ToVector2());

    public Vector2 WorldToScreen(Vector2 world) => world - Position;

    public Rectangle VisibleWorldBounds =>
        new((int)Position.X, (int)Position.Y, ViewportWidth, ViewportHeight);

    public void Follow(Vector2 worldFocus, float lerpFactor = 0.14f)
    {
        var desired = worldFocus - new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
        Position = Vector2.Lerp(Position, desired, Math.Clamp(lerpFactor, 0f, 1f));
        ClampToWorld();
    }

    public void ClampToWorld()
    {
        var maxX = Math.Max(0, WorldWidth - ViewportWidth);
        var maxY = Math.Max(0, WorldHeight - ViewportHeight);
        Position = new Vector2(
            Math.Clamp(Position.X, 0, maxX),
            Math.Clamp(Position.Y, 0, maxY));
    }
}
