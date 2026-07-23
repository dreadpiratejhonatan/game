using ModularGameEngine.Engine.Core;

namespace ModularGameEngine;

/// <summary>
/// Ponto de entrada do jogo.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        using var game = new GameEngine();
        game.Run();
    }
}
