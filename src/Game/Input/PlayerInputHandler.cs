using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ModularGameEngine.Engine.Core;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;
using ModularGameEngine.Game.Debug;
using ModularGameEngine.Game.Spawning;
using ModularGameEngine.Mods;

namespace ModularGameEngine.Game.Input;

/// <summary>
/// Input do jogador: mover, atacar, toggle debug.
/// Spawn aleatório só em modo debug (F1).
/// </summary>
public class PlayerInputHandler
{
    private readonly World _world;
    private readonly ModManager _mods;
    private readonly UnitFactory _unitFactory;
    private readonly Camera2D _camera;
    private readonly DebugState _debug;

    private MouseState _previousMouse;
    private KeyboardState _previousKeyboard;

    public PlayerInputHandler(
        World world,
        ModManager mods,
        UnitFactory unitFactory,
        Camera2D camera,
        DebugState debug)
    {
        _world = world;
        _mods = mods;
        _unitFactory = unitFactory;
        _camera = camera;
        _debug = debug;
    }

    /// <summary>Retorna true se o jogo deve encerrar (ESC).</summary>
    public bool Update(MouseState mouse, KeyboardState keyboard)
    {
        if (keyboard.IsKeyDown(Keys.Escape))
            return true;

        if (keyboard.IsKeyDown(Keys.F1) && !_previousKeyboard.IsKeyDown(Keys.F1))
        {
            _debug.Enabled = !_debug.Enabled;
            Console.WriteLine(_debug.Enabled
                ? "[DEBUG] ON — Espaço spawna unidades"
                : "[DEBUG] OFF — spawn desativado");
        }

        HandleClick(mouse);
        HandleDebugSpawn(keyboard, mouse);

        _previousMouse = mouse;
        _previousKeyboard = keyboard;
        return false;
    }

    private void HandleClick(MouseState mouse)
    {
        if (mouse.LeftButton != ButtonState.Pressed || _previousMouse.LeftButton == ButtonState.Pressed)
            return;

        var player = _world.GetEntities()
            .FirstOrDefault(e => e.HasComponent<PlayerControlledComponent>());
        if (player == null) return;

        var moveTarget = player.GetComponent<MoveTargetComponent>();
        var combat = player.GetComponent<CombatTargetComponent>();
        var render = player.GetComponent<RenderComponent>();
        var playerTeam = player.GetComponent<TeamComponent>();
        if (moveTarget == null || combat == null || render == null || playerTeam == null) return;

        var worldClick = _camera.ScreenToWorld(mouse.Position);
        var clickedEnemy = FindHostileAtWorldPoint(worldClick, playerTeam.Faction);

        if (clickedEnemy != null)
        {
            combat.Target = clickedEnemy;
            var enemyTransform = clickedEnemy.GetComponent<TransformComponent>();
            if (enemyTransform != null)
                moveTarget.Target = enemyTransform.Position;

            Console.WriteLine($"[PLAYER] Alvo: {clickedEnemy.GetComponent<UnitStatsComponent>()?.UnitType}");
        }
        else
        {
            combat.Target = null;
            var destination = worldClick - render.Size / 2f;
            destination.X = Math.Clamp(destination.X, 8, Math.Max(8, _camera.WorldWidth - render.Size.X - 8));
            destination.Y = Math.Clamp(destination.Y, 8, Math.Max(8, _camera.WorldHeight - render.Size.Y - 8));
            moveTarget.Target = destination;
        }
    }

    private void HandleDebugSpawn(KeyboardState keyboard, MouseState mouse)
    {
        if (!_debug.Enabled) return;
        if (!keyboard.IsKeyDown(Keys.Space) || _previousKeyboard.IsKeyDown(Keys.Space))
            return;

        var worldPos = _camera.ScreenToWorld(mouse.Position.ToVector2());
        _unitFactory.SpawnRandom(_mods, worldPos);
    }

    private Entity? FindHostileAtWorldPoint(Vector2 worldPoint, string playerFaction)
    {
        Entity? hit = null;
        var point = new Point((int)worldPoint.X, (int)worldPoint.Y);

        foreach (var entity in _world.GetEntities())
        {
            var team = entity.GetComponent<TeamComponent>();
            if (team == null || !TeamComponent.AreHostile(playerFaction, team.Faction))
                continue;

            var bounds = GetEntityHitbox(entity);
            if (bounds.HasValue && bounds.Value.Contains(point))
                hit = entity;
        }

        return hit;
    }

    private static Rectangle? GetEntityHitbox(Entity entity)
    {
        var transform = entity.GetComponent<TransformComponent>();
        var render = entity.GetComponent<RenderComponent>();
        if (transform == null || render == null) return null;

        const int pad = 8;
        return new Rectangle(
            (int)transform.Position.X - pad,
            (int)transform.Position.Y - pad - 16,
            (int)render.Size.X + pad * 2,
            (int)render.Size.Y + pad * 2 + 16);
    }
}
