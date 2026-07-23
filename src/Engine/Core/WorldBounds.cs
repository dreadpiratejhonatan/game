namespace ModularGameEngine.Engine.Core;

/// <summary>
/// Limites do mundo jogável. Pode ser atualizado ao carregar uma cena JSON.
/// </summary>
public class WorldBounds
{
    public int Width { get; set; } = 3200;
    public int Height { get; set; } = 2400;

    public void Set(int width, int height)
    {
        Width = Math.Max(640, width);
        Height = Math.Max(480, height);
    }
}
