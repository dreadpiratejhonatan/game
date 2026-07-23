using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Engine.Graphics;
using ModularGameEngine.Game.Bootstrap;
using ModularGameEngine.Game.Input;
using ModularGameEngine.Game.Spawning;
using ModularGameEngine.Game.Systems;
using ModularGameEngine.Mods;

namespace ModularGameEngine.Engine.Core;

/// <summary>
/// Orquestrador fino do game loop. Não contém lógica de gameplay —
/// delega para ModManager, UnitFactory, PlayerInputHandler e Systems.
/// </summary>
public class GameEngine : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    private World _world = null!;
    private ModManager _modManager = null!;
    private RenderSystem _renderSystem = null!;
    private UnitFactory _unitFactory = null!;
    private PlayerInputHandler _input = null!;
    private GameCursor _cursor = null!;

    public GameEngine()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false; // cursor customizado estilo RTS (StarCraft)

        _graphics.PreferredBackBufferWidth = SystemRegistrar.WorldWidth;
        _graphics.PreferredBackBufferHeight = SystemRegistrar.WorldHeight;
        _graphics.ApplyChanges();

        Window.Title = "Modular Game Engine - ARPG | Clique chão: mover | Clique inimigo: atacar | Espaço: spawn";
    }

    protected override void Initialize()
    {
        Console.WriteLine("=== MODULAR GAME ENGINE ===");
        Console.WriteLine("Controles:");
        Console.WriteLine("  Clique no chão     - mover personagem");
        Console.WriteLine("  Clique no inimigo  - perseguir e atacar");
        Console.WriteLine("  Espaço             - spawnar unidade aleatória");
        Console.WriteLine("  ESC                - sair");
        Console.WriteLine();

        _modManager = new ModManager("data");
        _modManager.LoadMods();

        _world = new World();
        SystemRegistrar.RegisterAll(_world);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        var spriteGenerator = new SpriteGenerator(GraphicsDevice);
        _renderSystem = new RenderSystem(GraphicsDevice, _spriteBatch, spriteGenerator);

        _unitFactory = new UnitFactory(_world, spriteGenerator);
        _input = new PlayerInputHandler(_world, _modManager, _unitFactory);
        _cursor = new GameCursor(GraphicsDevice);

        _unitFactory.SpawnInitialScene(
            _modManager,
            new Vector2(SystemRegistrar.WorldWidth / 2f, SystemRegistrar.WorldHeight / 2f));

        Console.WriteLine($"Mundo pronto. {_world.GetEntities().Count()} entidades | {_modManager.LoadedMods.Count} mod(s).");
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
        _cursor.Update(deltaTime, _world, mouse.Position);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(28, 32, 42));
        _renderSystem.Draw(_world.GetEntities(), null);

        // Cursor por cima de tudo (SpriteBatch próprio)
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _cursor.Draw(_spriteBatch, Mouse.GetState().Position);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
