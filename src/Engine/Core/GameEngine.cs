using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Engine.Graphics;
using ModularGameEngine.Game.Bootstrap;
using ModularGameEngine.Game.Debug;
using ModularGameEngine.Game.Input;
using ModularGameEngine.Game.Spawning;
using ModularGameEngine.Game.Systems;
using ModularGameEngine.Mods;

namespace ModularGameEngine.Engine.Core;

/// <summary>
/// Orquestrador fino do game loop.
/// </summary>
public class GameEngine : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    private World _world = null!;
    private ModManager _modManager = null!;
    private Camera2D _camera = null!;
    private DebugState _debug = null!;
    private RenderSystem _renderSystem = null!;
    private UnitFactory _unitFactory = null!;
    private PlayerInputHandler _input = null!;
    private GameCursor _cursor = null!;

    public GameEngine()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;

        _graphics.PreferredBackBufferWidth = SystemRegistrar.ViewportWidth;
        _graphics.PreferredBackBufferHeight = SystemRegistrar.ViewportHeight;
        _graphics.ApplyChanges();

        Window.Title = "Modular Game Engine | Clique: mover/atacar | F1: debug | ESC: sair";
    }

    protected override void Initialize()
    {
        Console.WriteLine("=== MODULAR GAME ENGINE ===");
        Console.WriteLine("Controles:");
        Console.WriteLine("  Clique no chão     - mover personagem");
        Console.WriteLine("  Clique no inimigo  - perseguir e atacar");
        Console.WriteLine("  F1                 - toggle debug (Espaço = spawn)");
        Console.WriteLine("  ESC                - sair");
        Console.WriteLine();

        _modManager = new ModManager("data");
        _modManager.LoadMods();

        _camera = new Camera2D
        {
            ViewportWidth = SystemRegistrar.ViewportWidth,
            ViewportHeight = SystemRegistrar.ViewportHeight,
            WorldWidth = SystemRegistrar.WorldWidth,
            WorldHeight = SystemRegistrar.WorldHeight,
            Position = new Vector2(
                SystemRegistrar.WorldWidth / 2f - SystemRegistrar.ViewportWidth / 2f,
                SystemRegistrar.WorldHeight / 2f - SystemRegistrar.ViewportHeight / 2f)
        };
        _camera.ClampToWorld();

        _debug = new DebugState();
        _world = new World();
        SystemRegistrar.RegisterAll(_world, _camera);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        var spriteGenerator = new SpriteGenerator(GraphicsDevice);
        _renderSystem = new RenderSystem(GraphicsDevice, _spriteBatch, spriteGenerator);

        _unitFactory = new UnitFactory(_world, spriteGenerator);
        _input = new PlayerInputHandler(_world, _modManager, _unitFactory, _camera, _debug);
        _cursor = new GameCursor(GraphicsDevice);

        _unitFactory.SpawnInitialScene(
            _modManager,
            new Vector2(SystemRegistrar.WorldWidth / 2f, SystemRegistrar.WorldHeight / 2f),
            SystemRegistrar.WorldWidth,
            SystemRegistrar.WorldHeight);

        Console.WriteLine(
            $"Mundo {_camera.WorldWidth}x{_camera.WorldHeight} | " +
            $"{_world.GetEntities().Count()} entidades | {_modManager.LoadedMods.Count} mod(s).");
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        var mouse = Mouse.GetState();

        if (_input.Update(mouse, keyboard))
        {
            Exit();
            return;
        }

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _world.Update(deltaTime);
        _renderSystem.Update(_world.GetEntities(), deltaTime);
        _cursor.Update(deltaTime, _world, _camera, mouse.Position);

        _debug.EntityCount = _world.GetEntities().Count();
        _debug.ModCount = _modManager.LoadedMods.Count;
        _debug.UnitDefCount = _modManager.UnitDefinitions.Count;
        _debug.CameraPosition = new Vector2Snapshot(_camera.Position.X, _camera.Position.Y);
        _debug.Fps = deltaTime > 0 ? 1f / deltaTime : 0f;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(28, 32, 42));
        _renderSystem.Draw(_world.GetEntities(), _camera, _debug);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _cursor.Draw(_spriteBatch, Mouse.GetState().Position);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
