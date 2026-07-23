using ModularGameEngine.Engine.Core;
using ModularGameEngine.Mods;

namespace ModularGameEngine;

/// <summary>
/// Ponto de entrada. Use --smoke para validar mods sem abrir a janela.
/// </summary>
public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Any(a => a.Equals("--smoke", StringComparison.OrdinalIgnoreCase)))
            return RunSmoke();

        using var game = new GameEngine();
        game.Run();
        return 0;
    }

    /// <summary>
    /// Smoke check headless: carrega mods e valida conteúdo mínimo.
    /// </summary>
    private static int RunSmoke()
    {
        Console.WriteLine("=== SMOKE CHECK ===");

        try
        {
            var mods = new ModManager("data");
            mods.LoadMods();

            if (mods.LoadedMods.Count < 1)
            {
                Console.Error.WriteLine("FAIL: nenhum mod carregado.");
                return 1;
            }

            if (mods.UnitDefinitions.Count < 1)
            {
                Console.Error.WriteLine("FAIL: nenhuma unidade registrada.");
                return 1;
            }

            if (mods.GetUnitDefinition("soldier_basic") == null)
            {
                Console.Error.WriteLine("FAIL: soldier_basic ausente no base_mod.");
                return 1;
            }

            Console.WriteLine($"SMOKE OK — {mods.LoadedMods.Count} mod(s), {mods.UnitDefinitions.Count} unidade(s).");
            foreach (var mod in mods.LoadedMods)
                Console.WriteLine($"  - {mod.Id} v{mod.Version}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAIL: {ex.Message}");
            return 1;
        }
    }
}
