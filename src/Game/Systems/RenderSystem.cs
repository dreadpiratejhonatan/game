using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Engine.Graphics;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

/// <summary>
/// Renderiza o mundo: terreno em tiles, sombras, sprites pixel-art animados,
/// barras de HP, indicadores de seleção e a caixa de seleção do mouse.
/// Unidades sem sprite caem no fallback de forma geométrica.
/// </summary>
public class RenderSystem : ISystem
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _pixelTexture;
    private readonly Texture2D _terrainTile;
    private readonly Texture2D _shadowTexture;
    private readonly Texture2D _particleTexture;
    
    private float _moveMarkerPulse = 0f;

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

    public void Draw(IEnumerable<Entity> entities, Rectangle? selectionBox)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        DrawTerrain();

        // Ordena por Y para dar noção de profundidade (quem está mais "embaixo" fica na frente)
        var entityList = entities
            .Where(e => e.HasComponent<TransformComponent>() && e.HasComponent<RenderComponent>())
            .OrderBy(e => e.GetComponent<TransformComponent>()!.Position.Y)
            .ToList();

        // Partículas (atrás das unidades)
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

        // Marcador de destino do personagem principal (pulsante)
        foreach (var entity in entityList)
        {
            var moveTarget = entity.GetComponent<MoveTargetComponent>();
            var render = entity.GetComponent<RenderComponent>();
            if (moveTarget?.Target == null || render == null) continue;
            if (!entity.HasComponent<PlayerControlledComponent>()) continue;

            var targetCenter = moveTarget.Target.Value + render.Size / 2f;
            DrawMoveMarkerAnimated(targetCenter);
        }

        foreach (var entity in entityList)
        {
            var transform = entity.GetComponent<TransformComponent>()!;
            var render = entity.GetComponent<RenderComponent>()!;

            var bounds = new Rectangle(
                (int)transform.Position.X, (int)transform.Position.Y,
                (int)render.Size.X, (int)render.Size.Y);

            var isSelected = entity.GetComponent<SelectableComponent>()?.IsSelected == true;
            var team = entity.GetComponent<TeamComponent>();
            var isEnemy = team?.Faction == "enemy";
            var isCombatTarget = entityList.Any(e =>
                e.GetComponent<CombatTargetComponent>()?.Target == entity
                && e.HasComponent<PlayerControlledComponent>());

            // Sombra no chão
            var shadowRect = new Rectangle(
                bounds.Center.X - (bounds.Width + 10) / 2,
                bounds.Bottom - 5,
                bounds.Width + 10,
                Math.Max(8, bounds.Width / 3));
            _spriteBatch.Draw(_shadowTexture, shadowRect, Color.White);

            // Anel: player (verde), inimigo (vermelho), alvo do player (vermelho forte)
            if (isSelected)
            {
                var ring = new Rectangle(bounds.X - 5, bounds.Bottom - 3, bounds.Width + 10, 7);
                DrawRectOutline(ring, Color.LimeGreen, 2);
            }
            else if (isCombatTarget)
            {
                var ring = new Rectangle(bounds.X - 6, bounds.Bottom - 3, bounds.Width + 12, 7);
                DrawRectOutline(ring, Color.OrangeRed, 2);
            }
            else if (isEnemy)
            {
                var ring = new Rectangle(bounds.X - 3, bounds.Bottom - 2, bounds.Width + 6, 5);
                DrawRectOutline(ring, Color.DarkRed * 0.7f, 1);
            }

            // Tint de hit flash
            var hitFlash = entity.GetComponent<CombatStateComponent>()?.HitFlashTimer > 0f;
            var tint = hitFlash ? Color.Lerp(Color.White, Color.Red, 0.65f) : Color.White;

            // Sprite animado (ou fallback geométrico)
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

            // Barra de HP: player, feridos, inimigos, ou alvo
            var stats = entity.GetComponent<UnitStatsComponent>();
            if (stats != null && (isSelected || isEnemy || isCombatTarget || stats.CurrentHealth < stats.MaxHealth))
            {
                DrawHealthBar(bounds, stats.CurrentHealth / stats.MaxHealth);
            }
        }

        // Caixa de seleção do mouse (por cima de tudo)
        if (selectionBox.HasValue)
        {
            _spriteBatch.Draw(_pixelTexture, selectionBox.Value, Color.LimeGreen * 0.12f);
            DrawRectOutline(selectionBox.Value, Color.LimeGreen, 1);
        }

        // UI na tela: barra de HP do player e contador de NPCs
        DrawUI(entityList);

        _spriteBatch.End();
    }

    private void DrawTerrain()
    {
        var viewport = _graphicsDevice.Viewport;
        for (int x = 0; x < viewport.Width; x += _terrainTile.Width)
            for (int y = 0; y < viewport.Height; y += _terrainTile.Height)
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
        const int thickness = 2;
        
        // Cruz com espessura
        for (int i = 0; i < thickness; i++)
        {
            _spriteBatch.Draw(_pixelTexture, 
                new Rectangle((int)center.X - arm, (int)center.Y - thickness/2 + i, arm * 2, 1), color);
            _spriteBatch.Draw(_pixelTexture, 
                new Rectangle((int)center.X - thickness/2 + i, (int)center.Y - arm, 1, arm * 2), color);
        }
        
        // Círculo externo pulsante
        var circleRadius = 10 + (int)(pulse * 3);
        DrawCircleOutline(center, circleRadius, color * 0.6f);
    }
    
    private void DrawCircleOutline(Vector2 center, int radius, Color color)
    {
        var points = 24;
        for (int i = 0; i < points; i++)
        {
            var angle = i * MathHelper.TwoPi / points;
            var x = center.X + (float)Math.Cos(angle) * radius;
            var y = center.Y + (float)Math.Sin(angle) * radius;
            _spriteBatch.Draw(_pixelTexture, new Rectangle((int)x, (int)y, 2, 2), color);
        }
    }
    
    private void DrawUI(List<Entity> entities)
    {
        var player = entities.FirstOrDefault(e => e.HasComponent<PlayerControlledComponent>());
        if (player == null) return;

        var stats = player.GetComponent<UnitStatsComponent>();
        if (stats == null) return;

        // Barra de HP do player (canto superior esquerdo)
        var barX = 20;
        var barY = 20;
        var barWidth = 200;
        var barHeight = 16;
        
        var bgRect = new Rectangle(barX - 2, barY - 2, barWidth + 4, barHeight + 4);
        _spriteBatch.Draw(_pixelTexture, bgRect, Color.Black * 0.7f);
        
        var fillWidth = (int)(barWidth * (stats.CurrentHealth / stats.MaxHealth));
        var hpRect = new Rectangle(barX, barY, fillWidth, barHeight);
        
        var hpColor = stats.CurrentHealth / stats.MaxHealth > 0.6f ? new Color(80, 220, 100)
                    : stats.CurrentHealth / stats.MaxHealth > 0.3f ? new Color(255, 200, 50)
                    : new Color(255, 80, 80);
        
        _spriteBatch.Draw(_pixelTexture, hpRect, hpColor);
        _spriteBatch.Draw(_pixelTexture, 
            new Rectangle(barX, barY, barWidth, barHeight), 
            Color.Transparent);
        DrawRectOutline(bgRect, Color.White * 0.8f, 1);
        
        // Contador de inimigos vivos (canto superior direito)
        var enemyCount = entities.Count(e => e.GetComponent<TeamComponent>()?.Faction == "enemy");
        var viewport = _graphicsDevice.Viewport;
        var counterX = viewport.Width - 170;
        var counterY = 20;

        var counterBg = new Rectangle(counterX, counterY, 150, 24);
        _spriteBatch.Draw(_pixelTexture, counterBg, Color.Black * 0.7f);
        DrawRectOutline(counterBg, Color.OrangeRed * 0.8f, 1);

        // Bloco vermelho proporcional (indicador visual simples sem fonte)
        var fill = Math.Clamp(enemyCount, 0, 10);
        if (fill > 0)
        {
            _spriteBatch.Draw(_pixelTexture,
                new Rectangle(counterX + 8, counterY + 6, fill * 13, 12),
                Color.OrangeRed);
        }
    }

    private void DrawMoveMarker(Vector2 center)
    {
        var color = Color.LimeGreen * 0.8f;
        const int arm = 6;
        _spriteBatch.Draw(_pixelTexture, new Rectangle((int)center.X - arm, (int)center.Y - 1, arm * 2, 2), color);
        _spriteBatch.Draw(_pixelTexture, new Rectangle((int)center.X - 1, (int)center.Y - arm, 2, arm * 2), color);
    }
}
