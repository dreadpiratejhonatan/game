using ModularGameEngine.Engine.Core;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Systems;

namespace ModularGameEngine.Game.Bootstrap;

/// <summary>
/// Registra os sistemas do jogo no World, em ordem de execução.
/// Novas features: adicione o sistema aqui (e só aqui).
/// </summary>
public static class SystemRegistrar
{
    /// <summary>Tamanho da janela (pixels).</summary>
    public const int ViewportWidth = 1280;
    public const int ViewportHeight = 720;

    /// <summary>Tamanho do mundo jogável (maior que a viewport → câmera).</summary>
    public const int WorldWidth = 3200;
    public const int WorldHeight = 2400;

    public static void RegisterAll(World world, Camera2D camera)
    {
        world.AddSystem(new MovementSystem(WorldWidth, WorldHeight));
        world.AddSystem(new WanderSystem(WorldWidth, WorldHeight));
        world.AddSystem(new AnimationSystem());
        world.AddSystem(new ParticleSystem());
        world.AddSystem(new CombatSystem(world));
        world.AddSystem(new CameraSystem(camera));
        // Futuro: world.AddSystem(new InventorySystem());
        // Futuro: world.AddSystem(new SquadCommandSystem());
    }
}
