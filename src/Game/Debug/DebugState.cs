namespace ModularGameEngine.Game.Debug;

/// <summary>
/// Estado global de debug. Ferramentas de desenvolvimento só funcionam com Enabled=true.
/// Toggle: F1 em runtime.
/// </summary>
public class DebugState
{
    public bool Enabled { get; set; }

    public int EntityCount { get; set; }
    public int ModCount { get; set; }
    public int UnitDefCount { get; set; }
    public Vector2Snapshot CameraPosition { get; set; }
    public float Fps { get; set; }
}

/// <summary>Evita acoplar DebugState ao MonoGame Vector2 nos overlays simples.</summary>
public readonly record struct Vector2Snapshot(float X, float Y);
