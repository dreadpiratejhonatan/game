using ModularGameEngine.Engine.Core;
using ModularGameEngine.Mods;

namespace ModularGameEngine;

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

            if (mods.GetUnitDefinition("soldier_basic") == null)
            {
                Console.Error.WriteLine("FAIL: soldier_basic ausente no base_mod.");
                return 1;
            }

            var scene = mods.GetScene("baseline");
            if (scene == null)
            {
                Console.Error.WriteLine("FAIL: cena baseline ausente.");
                return 1;
            }

            var missing = mods.ValidateScene(scene);
            if (missing.Count > 0)
            {
                Console.Error.WriteLine($"FAIL: cena baseline referencia unidades inexistentes: {string.Join(", ", missing)}");
                return 1;
            }

            Console.WriteLine(
                $"SMOKE OK — {mods.LoadedMods.Count} mod(s), {mods.UnitDefinitions.Count} unidade(s), {mods.Scenes.Count} cena(s).");
            Console.WriteLine($"  scene: {scene.Id} ({scene.Spawns.Count} spawns)");
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
