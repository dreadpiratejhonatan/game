using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModularGameEngine.Engine.Core;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Engine.Graphics;
using ModularGameEngine.Game.Components;
using ModularGameEngine.Game.Debug;

namespace ModularGameEngine.Game.Systems;

/// <summary>
/// Renderiza o mundo (com câmera) e a UI em espaço de tela.
/// </summary>
public class RenderSystem : ISystem
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _pixelTexture;
    private readonly Texture2D _terrainTile;
    private readonly Texture2D _shadowTexture;
    private readonly Texture2D _particleTexture;

    private float _moveMarkerPulse;

    public RenderSystem(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, SpriteGenerator spriteGenerator)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;

        _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });

        _terrainTile = spriteGenerator.CreateTerrainTile();
        _shadowTexture = spriteGenerator.CreateShadow();
        _particleTexture = spriteGenerator.CreateParticle();
    }

    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        _moveMarkerPulse += deltaTime * 4f;
    }

    public void Draw(IEnumerable<Entity> entities, Camera2D camera, DebugState debug)
    {
        // --- Mundo (afetado pela câmera) ---
        _spriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: camera.GetViewMatrix());

        DrawTerrain(camera);

        var entityList = entities
            .Where(e => e.HasComponent<TransformComponent>() && e.HasComponent<RenderComponent>())
            .OrderBy(e => e.GetComponent<TransformComponent>()!.Position.Y)
            .ToList();

        foreach (var entity in entityList)
        {
            var emitter = entity.GetComponent<ParticleEmitterComponent>();
            if (emitter == null) continue;

            foreach (var particle in emitter.Particles)
            {
                var rect = new Rectangle(
                    (int)particle.Position.X - 1,
                    (int)particle.Position.Y - 1,
                    3, 3);
                _spriteBatch.Draw(_particleTexture, rect, particle.Color);
            }
        }

        foreach (var entity in entityList)
        {
            var moveTarget = entity.GetComponent<MoveTargetComponent>();
            var render = entity.GetComponent<RenderComponent>();
            if (moveTarget?.Target == null || render == null) continue;
            if (!entity.HasComponent<PlayerControlledComponent>()) continue;

            DrawMoveMarkerAnimated(moveTarget.Target.Value + render.Size / 2f);
        }

        foreach (var entity in entityList)
        {
            var transform = entity.GetComponent<TransformComponent>()!;
            var render = entity.GetComponent<RenderComponent>()!;

            var bounds = new Rectangle(
                (int)transform.Position.X, (int)transform.Position.Y,
                (int)render.Size.X, (int)render.Size.Y);

            // Culling simples
            var cullBounds = bounds;
            cullBounds.Inflate(48, 48);
            if (!camera.VisibleWorldBounds.Intersects(cullBounds))
                continue;

            var isSelected = entity.GetComponent<SelectableComponent>()?.IsSelected == true;
            var team = entity.GetComponent<TeamComponent>();
            var isEnemy = team?.Faction == "enemy";
            var isCombatTarget = entityList.Any(e =>
                e.GetComponent<CombatTargetComponent>()?.Target == entity
                && e.HasComponent<PlayerControlledComponent>());

            var shadowRect = new Rectangle(
                bounds.Center.X - (bounds.Width + 10) / 2,
                bounds.Bottom - 5,
                bounds.Width + 10,
                Math.Max(8, bounds.Width / 3));
            _spriteBatch.Draw(_shadowTexture, shadowRect, Color.White);

            if (isSelected)
                DrawRectOutline(new Rectangle(bounds.X - 5, bounds.Bottom - 3, bounds.Width + 10, 7), Color.LimeGreen, 2);
            else if (isCombatTarget)
                DrawRectOutline(new Rectangle(bounds.X - 6, bounds.Bottom - 3, bounds.Width + 12, 7), Color.OrangeRed, 2);
            else if (isEnemy)
                DrawRectOutline(new Rectangle(bounds.X - 3, bounds.Bottom - 2, bounds.Width + 6, 5), Color.DarkRed * 0.7f, 1);

            var hitFlash = entity.GetComponent<CombatStateComponent>()?.HitFlashTimer > 0f;
            var tint = hitFlash ? Color.Lerp(Color.White, Color.Red, 0.65f) : Color.White;

            var sprite = entity.GetComponent<SpriteComponent>();
            if (sprite != null && sprite.Frames.Length > 0)
            {
                var frame = sprite.Frames[Math.Clamp(sprite.CurrentFrame, 0, sprite.Frames.Length - 1)];
                var destHeight = (int)(bounds.Height * 1.6f);
                var destWidth = destHeight * frame.Width / frame.Height;
                var dest = new Rectangle(
                    bounds.Center.X - destWidth / 2,
                    bounds.Bottom - destHeight,
                    destWidth, destHeight);

                _spriteBatch.Draw(
                    frame, dest, null, tint, 0f, Vector2.Zero,
                    sprite.FacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            else
            {
                _spriteBatch.Draw(_pixelTexture, bounds, hitFlash ? Color.Red : render.Color);
            }

            var stats = entity.GetComponent<UnitStatsComponent>();
            if (stats != null && (isSelected || isEnemy || isCombatTarget || stats.CurrentHealth < stats.MaxHealth))
                DrawHealthBar(bounds, stats.CurrentHealth / stats.MaxHealth);
        }

        _spriteBatch.End();

        // --- UI em espaço de tela (sem câmera) ---
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        DrawUI(entityList, debug);
        _spriteBatch.End();
    }

    private void DrawTerrain(Camera2D camera)
    {
        var tile = _terrainTile.Width;
        var view = camera.VisibleWorldBounds;
        var startX = (view.X / tile) * tile - tile;
        var startY = (view.Y / tile) * tile - tile;
        var endX = view.Right + tile;
        var endY = view.Bottom + tile;

        for (int x = startX; x < endX; x += tile)
            for (int y = startY; y < endY; y += tile)
                _spriteBatch.Draw(_terrainTile, new Vector2(x, y), Color.White);
    }

    private void DrawRectOutline(Rectangle rect, Color color, int thickness)
    {
        _spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        _spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color);
        _spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        _spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color);
    }

    private void DrawHealthBar(Rectangle unitBounds, float healthPercent)
    {
        var barWidth = Math.Max(unitBounds.Width, 24);
        var barX = unitBounds.Center.X - barWidth / 2;
        var barY = unitBounds.Y - (int)(unitBounds.Height * 0.7f) - 6;

        var background = new Rectangle(barX, barY, barWidth, 4);
        var fill = new Rectangle(barX, barY, (int)(barWidth * Math.Clamp(healthPercent, 0f, 1f)), 4);

        var fillColor = healthPercent > 0.6f ? Color.LimeGreen
                      : healthPercent > 0.3f ? Color.Gold
                      : Color.Red;

        _spriteBatch.Draw(_pixelTexture, background, Color.Black * 0.6f);
        _spriteBatch.Draw(_pixelTexture, fill, fillColor);
    }

    private void DrawMoveMarkerAnimated(Vector2 center)
    {
        var pulse = (float)Math.Sin(_moveMarkerPulse) * 0.3f + 0.7f;
        var color = Color.LimeGreen * pulse;
        const int arm = 7;

        for (int i = 0; i < 2; i++)
        {
            _spriteBatch.Draw(_pixelTexture,
                new Rectangle((int)center.X - arm, (int)center.Y - 1 + i, arm * 2, 1), color);
            _spriteBatch.Draw(_pixelTexture,
                new Rectangle((int)center.X - 1 + i, (int)center.Y - arm, 1, arm * 2), color);
        }

        DrawCircleOutline(center, 10 + (int)(pulse * 3), color * 0.6f);
    }

    private void DrawCircleOutline(Vector2 center, int radius, Color color)
    {
        const int points = 24;
        for (int i = 0; i < points; i++)
        {
            var angle = i * MathHelper.TwoPi / points;
            var x = center.X + (float)Math.Cos(angle) * radius;
            var y = center.Y + (float)Math.Sin(angle) * radius;
            _spriteBatch.Draw(_pixelTexture, new Rectangle((int)x, (int)y, 2, 2), color);
        }
    }

    private void DrawUI(List<Entity> entities, DebugState debug)
    {
        var player = entities.FirstOrDefault(e => e.HasComponent<PlayerControlledComponent>());
        if (player != null)
        {
            var stats = player.GetComponent<UnitStatsComponent>();
            if (stats != null)
            {
                var barX = 20;
                var barY = 20;
                var barWidth = 200;
                var barHeight = 16;

                var bgRect = new Rectangle(barX - 2, barY - 2, barWidth + 4, barHeight + 4);
                _spriteBatch.Draw(_pixelTexture, bgRect, Color.Black * 0.7f);

                var fillWidth = (int)(barWidth * (stats.CurrentHealth / stats.MaxHealth));
                var hpColor = stats.CurrentHealth / stats.MaxHealth > 0.6f ? new Color(80, 220, 100)
                            : stats.CurrentHealth / stats.MaxHealth > 0.3f ? new Color(255, 200, 50)
                            : new Color(255, 80, 80);

                _spriteBatch.Draw(_pixelTexture, new Rectangle(barX, barY, fillWidth, barHeight), hpColor);
                DrawRectOutline(bgRect, Color.White * 0.8f, 1);
            }
        }

        var enemyCount = entities.Count(e => e.GetComponent<TeamComponent>()?.Faction == "enemy");
        var counterX = _graphicsDevice.Viewport.Width - 170;
        var counterBg = new Rectangle(counterX, 20, 150, 24);
        _spriteBatch.Draw(_pixelTexture, counterBg, Color.Black * 0.7f);
        DrawRectOutline(counterBg, Color.OrangeRed * 0.8f, 1);

        var fill = Math.Clamp(enemyCount, 0, 10);
        if (fill > 0)
            _spriteBatch.Draw(_pixelTexture, new Rectangle(counterX + 8, 26, fill * 13, 12), Color.OrangeRed);

        if (debug.Enabled)
            DrawDebugOverlay(debug, enemyCount, entities.Count);
    }

    private void DrawDebugOverlay(DebugState debug, int enemyCount, int entityCount)
    {
        var panel = new Rectangle(20, 52, 280, 86);
        _spriteBatch.Draw(_pixelTexture, panel, Color.Black * 0.75f);
        DrawRectOutline(panel, Color.Yellow * 0.9f, 1);

        // Indicador "DEBUG" (bloco amarelo)
        _spriteBatch.Draw(_pixelTexture, new Rectangle(28, 60, 64, 10), Color.Gold);

        // Barras de telemetria simples (sem fonte bitmap)
        var fpsBar = Math.Clamp((int)(debug.Fps / 2f), 0, 60);
        _spriteBatch.Draw(_pixelTexture, new Rectangle(28, 80, fpsBar * 4, 6), Color.Cyan);

        var entBar = Math.Clamp(entityCount, 0, 40);
        _spriteBatch.Draw(_pixelTexture, new Rectangle(28, 94, entBar * 6, 6), Color.LightGreen);

        var modBar = Math.Clamp(debug.ModCount, 0, 10);
        _spriteBatch.Draw(_pixelTexture, new Rectangle(28, 108, modBar * 20, 6), Color.Orange);

        var enemyBar = Math.Clamp(enemyCount, 0, 20);
        _spriteBatch.Draw(_pixelTexture, new Rectangle(28, 122, enemyBar * 10, 6), Color.OrangeRed);
    }
}
