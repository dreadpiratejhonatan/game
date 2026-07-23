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
/// Cria entidades a partir de UnitDefinition (data-driven).
/// Ponto único de spawn — novas features de criação de unidade passam por aqui.
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
    /// Spawna o player no centro e algumas unidades de cada tipo definido nos mods.
    /// </summary>
    public void SpawnInitialScene(ModManager mods, Vector2 playerPosition, int copiesPerUnitType = 2)
    {
        var playerDef = mods.GetUnitDefinition("soldier_basic")
                        ?? mods.UnitDefinitions.FirstOrDefault();

        if (playerDef != null)
        {
            Spawn(playerDef, playerPosition, isPlayer: true);
            Console.WriteLine($"[PLAYER] {playerDef.Name} spawned.");
        }

        foreach (var unitDef in mods.UnitDefinitions)
        {
            for (int i = 0; i < copiesPerUnitType; i++)
            {
                var position = new Vector2(_random.Next(100, 1180), _random.Next(100, 620));
                Spawn(unitDef, position, isPlayer: false);
            }
        }
    }

    public Entity Spawn(UnitDefinition unitDef, Vector2 position, bool isPlayer = false)
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
        entity.AddComponent(new TeamComponent(isPlayer ? "player" : unitDef.Faction));

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
