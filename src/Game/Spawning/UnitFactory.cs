using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Engine.Graphics;
using ModularGameEngine.Engine.Util;
using ModularGameEngine.Game.Components;
using ModularGameEngine.Mods;
using ModularGameEngine.Mods.Models;

namespace ModularGameEngine.Game.Spawning;

/// <summary>
/// Cria entidades a partir de UnitDefinition / cenas JSON.
/// </summary>
public class UnitFactory
{
    private readonly World _world;
    private readonly SpriteGenerator _spriteGenerator;
    private readonly Dictionary<string, Texture2D[]> _spriteCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Random _random = new();

    public UnitFactory(World world, SpriteGenerator spriteGenerator)
    {
        _world = world;
        _spriteGenerator = spriteGenerator;
    }

    /// <summary>
    /// Spawna a cena baseline (ou fallback aleatório se a cena não existir).
    /// </summary>
    public SceneDefinition? SpawnScene(ModManager mods, string sceneId = "baseline")
    {
        var scene = mods.GetScene(sceneId);
        if (scene == null)
        {
            Console.WriteLine($"[SCENE] AVISO: cena '{sceneId}' não encontrada — fallback aleatório.");
            SpawnRandomFallback(mods, 3200, 2400);
            return null;
        }

        var missing = mods.ValidateScene(scene);
        if (missing.Count > 0)
        {
            Console.WriteLine($"[SCENE] AVISO: unidades ausentes na cena: {string.Join(", ", missing)}");
        }

        Console.WriteLine($"[SCENE] Carregando '{scene.Name}' ({scene.Id}) — {scene.WorldWidth}x{scene.WorldHeight}");

        if (scene.Player != null)
        {
            var playerDef = mods.GetUnitDefinition(scene.Player.UnitId);
            if (playerDef != null)
            {
                Spawn(playerDef, new Vector2(scene.Player.X, scene.Player.Y), isPlayer: true);
                Console.WriteLine($"[PLAYER] {playerDef.Name} at ({scene.Player.X:0}, {scene.Player.Y:0})");
            }
        }

        var spawned = 0;
        foreach (var actor in scene.Spawns)
        {
            var def = mods.GetUnitDefinition(actor.UnitId);
            if (def == null) continue;

            Spawn(def, new Vector2(actor.X, actor.Y), isPlayer: false, factionOverride: actor.Faction);
            spawned++;
        }

        Console.WriteLine($"[SCENE] {spawned} NPCs posicionados.");
        return scene;
    }

    private void SpawnRandomFallback(ModManager mods, int worldWidth, int worldHeight)
    {
        var playerDef = mods.GetUnitDefinition("soldier_basic") ?? mods.UnitDefinitions.FirstOrDefault();
        if (playerDef != null)
            Spawn(playerDef, new Vector2(worldWidth / 2f, worldHeight / 2f), isPlayer: true);

        var margin = 80;
        foreach (var unitDef in mods.UnitDefinitions)
        {
            for (int i = 0; i < 2; i++)
            {
                Spawn(unitDef, new Vector2(
                    _random.Next(margin, worldWidth - margin),
                    _random.Next(margin, worldHeight - margin)));
            }
        }
    }

    public Entity Spawn(
        UnitDefinition unitDef,
        Vector2 position,
        bool isPlayer = false,
        string? factionOverride = null)
    {
        var entity = _world.CreateEntity();

        entity.AddComponent(new TransformComponent(position));
        entity.AddComponent(new UnitStatsComponent(
            unitDef.Id,
            unitDef.Stats.MaxHealth,
            unitDef.Stats.MoveSpeed,
            unitDef.Stats.AttackDamage,
            unitDef.Stats.AttackRange,
            unitDef.Stats.AttackCooldown,
            unitDef.Stats.DetectionRange));

        var color = ColorUtil.ParseHex(unitDef.Visual.Color);
        var size = new Vector2(unitDef.Visual.Width, unitDef.Visual.Height);
        var shape = unitDef.Visual.Shape.Equals("circle", StringComparison.OrdinalIgnoreCase)
            ? RenderShape.Circle
            : RenderShape.Rectangle;

        entity.AddComponent(new RenderComponent(size, color, shape));
        entity.AddComponent(new MoveTargetComponent());
        entity.AddComponent(new SpriteComponent(GetOrCreateSprites(unitDef, color, shape)));
        entity.AddComponent(new ParticleEmitterComponent());
        entity.AddComponent(new CombatTargetComponent());
        entity.AddComponent(new CombatStateComponent());

        var faction = isPlayer
            ? "player"
            : (factionOverride ?? unitDef.Faction);
        entity.AddComponent(new TeamComponent(faction));

        if (isPlayer)
        {
            entity.AddComponent(new PlayerControlledComponent());
            entity.AddComponent(new SelectableComponent { IsSelected = true });
        }
        else
        {
            entity.AddComponent(new WanderComponent { Cooldown = (float)_random.NextDouble() * 2f });
        }

        return entity;
    }

    public Entity? SpawnRandom(ModManager mods, Vector2 position)
    {
        var def = mods.GetRandomUnitDefinition(_random);
        if (def == null) return null;

        var entity = Spawn(def, position, isPlayer: false);
        Console.WriteLine($"  + Spawned: {def.Name} ({def.Faction}) at ({position.X:0}, {position.Y:0})");
        return entity;
    }

    private Texture2D[] GetOrCreateSprites(UnitDefinition unitDef, Color color, RenderShape shape)
    {
        if (_spriteCache.TryGetValue(unitDef.Id, out var cached))
            return cached;

        Texture2D[] frames;
        if (shape == RenderShape.Circle)
            frames = _spriteGenerator.CreateBlobFrames(color);
        else if (unitDef.Visual.Width >= 40)
            frames = _spriteGenerator.CreateVehicleFrames(color);
        else
            frames = _spriteGenerator.CreateHumanoidFrames(color, hasWeapon: true);

        _spriteCache[unitDef.Id] = frames;
        return frames;
    }
}
