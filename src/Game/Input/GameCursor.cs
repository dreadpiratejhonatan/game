using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModularGameEngine.Engine.Core;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Input;

public enum CursorMode
{
    /// <summary>Cursor padrão / mover (verde, estilo comando RTS).</summary>
    Command,
    /// <summary>Sobre inimigo hostil (vermelho, mira de ataque).</summary>
    Attack
}

/// <summary>
/// Cursor customizado estilo StarCraft: mira verde para comando/movimento,
/// mira vermelha com cantoneiras quando o mouse está sobre um inimigo.
/// </summary>
public class GameCursor
{
    private readonly Texture2D _pixel;
    private readonly Texture2D _commandCursor;
    private readonly Texture2D _attackCursor;
    private float _pulse;

    public CursorMode Mode { get; private set; } = CursorMode.Command;

    public GameCursor(GraphicsDevice device)
    {
        _pixel = new Texture2D(device, 1, 1);
        _pixel.SetData(new[] { Color.White });
        _commandCursor = BuildCommandCursor(device);
        _attackCursor = BuildAttackCursor(device);
    }

    public void Update(float deltaTime, World world, Camera2D camera, Point mousePosition)
    {
        _pulse += deltaTime * 6f;
        var worldPoint = camera.ScreenToWorld(mousePosition);
        Mode = IsOverHostile(world, worldPoint) ? CursorMode.Attack : CursorMode.Command;
    }

    public void Draw(SpriteBatch spriteBatch, Point mousePosition)
    {
        var texture = Mode == CursorMode.Attack ? _attackCursor : _commandCursor;
        var origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
        var pulse = 1f + MathF.Sin(_pulse) * 0.06f;

        // Sombra suave atrás da mira (legibilidade no terreno)
        spriteBatch.Draw(
            texture,
            mousePosition.ToVector2() + new Vector2(1, 1),
            null,
            Color.Black * 0.45f,
            0f,
            origin,
            pulse,
            SpriteEffects.None,
            0f);

        spriteBatch.Draw(
            texture,
            mousePosition.ToVector2(),
            null,
            Color.White,
            0f,
            origin,
            pulse,
            SpriteEffects.None,
            0f);

        // Anel externo pulsante só no modo ataque (feedback agressivo)
        if (Mode == CursorMode.Attack)
        {
            var radius = 14 + (int)(MathF.Sin(_pulse) * 2);
            DrawCircle(spriteBatch, mousePosition.ToVector2(), radius, new Color(255, 60, 60) * 0.55f);
        }
    }

    private static bool IsOverHostile(World world, Vector2 worldPoint)
    {
        var player = world.GetEntities()
            .FirstOrDefault(e => e.HasComponent<PlayerControlledComponent>());
        var playerFaction = player?.GetComponent<TeamComponent>()?.Faction ?? "player";
        var point = new Point((int)worldPoint.X, (int)worldPoint.Y);

        foreach (var entity in world.GetEntities())
        {
            var team = entity.GetComponent<TeamComponent>();
            if (team == null || !TeamComponent.AreHostile(playerFaction, team.Faction))
                continue;

            var bounds = GetEntityHitbox(entity);
            if (bounds.HasValue && bounds.Value.Contains(point))
                return true;
        }

        return false;
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

    private void DrawCircle(SpriteBatch spriteBatch, Vector2 center, int radius, Color color)
    {
        const int points = 28;
        for (int i = 0; i < points; i++)
        {
            var angle = i * MathHelper.TwoPi / points;
            var x = center.X + MathF.Cos(angle) * radius;
            var y = center.Y + MathF.Sin(angle) * radius;
            spriteBatch.Draw(_pixel, new Rectangle((int)x, (int)y, 2, 2), color);
        }
    }

    /// <summary>Mira verde com cruz + cantoneiras (comando / move).</summary>
    private static Texture2D BuildCommandCursor(GraphicsDevice device)
    {
        const int s = 29;
        var px = new Color[s * s];
        var green = new Color(80, 255, 90);
        var dark = new Color(20, 80, 30);

        void Set(int x, int y, Color c)
        {
            if (x >= 0 && x < s && y >= 0 && y < s) px[y * s + x] = c;
        }

        var mid = s / 2;

        // Cruz central
        for (int i = 4; i <= s - 5; i++)
        {
            if (Math.Abs(i - mid) < 2) continue; // buraco no centro
            Set(i, mid, green);
            Set(i, mid - 1, dark);
            Set(mid, i, green);
            Set(mid - 1, i, dark);
        }

        // Cantoneiras nos 4 cantos (estilo reticula RTS)
        DrawCorner(Set, 2, 2, 1, 1, green, dark);
        DrawCorner(Set, s - 3, 2, -1, 1, green, dark);
        DrawCorner(Set, 2, s - 3, 1, -1, green, dark);
        DrawCorner(Set, s - 3, s - 3, -1, -1, green, dark);

        // Ponto central
        Set(mid, mid, green);

        var tex = new Texture2D(device, s, s);
        tex.SetData(px);
        return tex;
    }

    /// <summary>Mira vermelha mais agressiva (ataque).</summary>
    private static Texture2D BuildAttackCursor(GraphicsDevice device)
    {
        const int s = 31;
        var px = new Color[s * s];
        var red = new Color(255, 70, 55);
        var dark = new Color(90, 15, 10);

        void Set(int x, int y, Color c)
        {
            if (x >= 0 && x < s && y >= 0 && y < s) px[y * s + x] = c;
        }

        var mid = s / 2;

        // Cruz mais grossa
        for (int i = 3; i <= s - 4; i++)
        {
            if (Math.Abs(i - mid) <= 2) continue;
            Set(i, mid, red);
            Set(i, mid - 1, red);
            Set(i, mid + 1, dark);
            Set(mid, i, red);
            Set(mid - 1, i, red);
            Set(mid + 1, i, dark);
        }

        DrawCorner(Set, 1, 1, 1, 1, red, dark);
        DrawCorner(Set, s - 2, 1, -1, 1, red, dark);
        DrawCorner(Set, 1, s - 2, 1, -1, red, dark);
        DrawCorner(Set, s - 2, s - 2, -1, -1, red, dark);

        // Centro aberto (não cobre o alvo)
        Set(mid, mid, Color.Transparent);

        var tex = new Texture2D(device, s, s);
        tex.SetData(px);
        return tex;
    }

    private static void DrawCorner(
        Action<int, int, Color> set,
        int ox, int oy, int dx, int dy,
        Color color, Color outline)
    {
        for (int i = 0; i < 7; i++)
        {
            set(ox + i * dx, oy, color);
            set(ox + i * dx, oy - dy, outline);
            set(ox, oy + i * dy, color);
            set(ox - dx, oy + i * dy, outline);
        }
    }
}
