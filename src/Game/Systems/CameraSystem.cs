using ModularGameEngine.Engine.Core;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

/// <summary>
/// Mantém a câmera centrada no personagem principal (lerp suave + clamp no mundo).
/// </summary>
public class CameraSystem : ISystem
{
    private readonly Camera2D _camera;

    public CameraSystem(Camera2D camera)
    {
        _camera = camera;
    }

    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        var player = entities.FirstOrDefault(e => e.HasComponent<PlayerControlledComponent>());
        if (player == null) return;

        var transform = player.GetComponent<TransformComponent>();
        var render = player.GetComponent<RenderComponent>();
        if (transform == null) return;

        var focus = transform.Position;
        if (render != null)
            focus += render.Size * 0.5f;

        // Lerp um pouco mais responsivo em frames longos
        var t = 1f - MathF.Pow(0.001f, deltaTime);
        _camera.Follow(focus, Math.Clamp(t, 0.08f, 0.35f));
    }
}
