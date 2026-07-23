using ModularGameEngine.Engine.Core;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Systems;

namespace ModularGameEngine.Game.Bootstrap;

/// <summary>
/// Registra os sistemas do jogo no World, em ordem de execução.
/// </summary>
public static class SystemRegistrar
{
    public const int ViewportWidth = 1280;
    public const int ViewportHeight = 720;

    public const int DefaultWorldWidth = 3200;
    public const int DefaultWorldHeight = 2400;

    public static void RegisterAll(World world, Camera2D camera, WorldBounds bounds)
    {
        world.AddSystem(new MovementSystem(bounds));
        world.AddSystem(new WanderSystem(bounds));
        world.AddSystem(new AnimationSystem());
        world.AddSystem(new ParticleSystem());
        world.AddSystem(new CombatSystem(world));
        world.AddSystem(new CameraSystem(camera));
    }
}
