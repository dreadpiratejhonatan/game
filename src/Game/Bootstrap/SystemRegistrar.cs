using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Systems;

namespace ModularGameEngine.Game.Bootstrap;

/// <summary>
/// Registra os sistemas do jogo no World, em ordem de execução.
/// Novas features: adicione o sistema aqui (e só aqui).
/// </summary>
public static class SystemRegistrar
{
    public const int WorldWidth = 1280;
    public const int WorldHeight = 720;

    public static void RegisterAll(World world)
    {
        world.AddSystem(new MovementSystem());
        world.AddSystem(new WanderSystem(WorldWidth, WorldHeight));
        world.AddSystem(new AnimationSystem());
        world.AddSystem(new ParticleSystem());
        world.AddSystem(new CombatSystem(world));
        // Futuro: world.AddSystem(new CameraSystem());
        // Futuro: world.AddSystem(new InventorySystem());
        // Futuro: world.AddSystem(new SquadCommandSystem());
    }
}
