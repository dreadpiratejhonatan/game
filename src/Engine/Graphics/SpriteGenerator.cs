using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ModularGameEngine.Engine.Graphics;

/// <summary>
/// Gera texturas pixel-art proceduralmente em runtime.
/// Como o jogo ainda não tem assets de arte, os sprites são desenhados em código,
/// usando a cor definida no JSON do mod — mantendo o pipeline data-driven.
/// Cada unidade recebe 3 frames: [0] parado, [1] e [2] alternando na caminhada.
/// </summary>
public class SpriteGenerator
{
    private readonly GraphicsDevice _device;
    private readonly Random _random = new(12345); // seed fixa: terreno igual entre execuções

    public SpriteGenerator(GraphicsDevice device)
    {
        _device = device;
    }

    // ------------------------------------------------------------------
    // HUMANOIDE (soldado, arqueiro): 12x16 px
    // ------------------------------------------------------------------
    public Texture2D[] CreateHumanoidFrames(Color baseColor, bool hasWeapon)
    {
        return new[]
        {
            CreateHumanoid(baseColor, 0, hasWeapon),
            CreateHumanoid(baseColor, 1, hasWeapon),
            CreateHumanoid(baseColor, 2, hasWeapon)
        };
    }

    private Texture2D CreateHumanoid(Color baseColor, int frame, bool hasWeapon)
    {
        const int w = 12, h = 16;
        var px = new Color[w * h];

        var dark = Darken(baseColor, 0.55f);
        var light = Color.Lerp(baseColor, Color.White, 0.45f);
        var skin = new Color(224, 188, 148);
        var gray = new Color(158, 158, 158);

        void Set(int x, int y, Color c)
        {
            if (x >= 0 && x < w && y >= 0 && y < h) px[y * w + x] = c;
        }

        // Capacete + cabeça
        for (int x = 4; x <= 7; x++) Set(x, 0, dark);
        for (int y = 1; y <= 2; y++)
            for (int x = 4; x <= 7; x++) Set(x, y, skin);
        Set(6, 2, Color.Black * 0.8f); // olho (vira para o lado do flip)

        // Pescoço
        Set(5, 3, skin); Set(6, 3, skin);

        // Torso
        for (int y = 4; y <= 8; y++)
            for (int x = 3; x <= 8; x++) Set(x, y, baseColor);
        for (int x = 3; x <= 8; x++) Set(x, 9, dark);          // cinto
        for (int x = 4; x <= 7; x++) Set(x, 4, light);          // ombros iluminados

        // Braços
        for (int y = 4; y <= 8; y++) { Set(2, y, dark); Set(9, y, dark); }

        // Arma (lança/rifle simplificado)
        if (hasWeapon)
        {
            for (int y = 1; y <= 8; y++) Set(10, y, gray);
            Set(10, 1, light); // ponta
        }

        // Pernas: 3 poses de caminhada
        switch (frame)
        {
            case 0: // parado
                DrawLeg(Set, 4, 10, 14, dark);
                DrawLeg(Set, 7, 10, 14, dark);
                break;
            case 1: // passo aberto
                DrawLeg(Set, 3, 10, 14, dark);
                DrawLeg(Set, 8, 10, 14, dark);
                break;
            case 2: // pernas cruzando
                DrawLeg(Set, 5, 10, 14, dark);
                DrawLeg(Set, 6, 10, 14, dark);
                break;
        }

        // Pés
        Set(frame == 1 ? 3 : frame == 2 ? 5 : 4, 15, Color.Black * 0.85f);
        Set(frame == 1 ? 8 : frame == 2 ? 6 : 7, 15, Color.Black * 0.85f);

        return MakeTexture(w, h, px);
    }

    private static void DrawLeg(Action<int, int, Color> set, int x, int yStart, int yEnd, Color color)
    {
        for (int y = yStart; y <= yEnd; y++) set(x, y, color);
    }

    // ------------------------------------------------------------------
    // VEÍCULO (tanque): 16x12 px com esteira animada
    // ------------------------------------------------------------------
    public Texture2D[] CreateVehicleFrames(Color baseColor)
    {
        return new[] { CreateVehicle(baseColor, 0), CreateVehicle(baseColor, 1), CreateVehicle(baseColor, 0) };
    }

    private Texture2D CreateVehicle(Color baseColor, int trackOffset)
    {
        const int w = 16, h = 12;
        var px = new Color[w * h];

        var dark = Darken(baseColor, 0.5f);
        var light = Color.Lerp(baseColor, Color.White, 0.35f);
        var gray = new Color(120, 120, 120);

        void Set(int x, int y, Color c)
        {
            if (x >= 0 && x < w && y >= 0 && y < h) px[y * w + x] = c;
        }

        // Torre
        for (int y = 1; y <= 3; y++)
            for (int x = 4; x <= 9; x++) Set(x, y, light);

        // Canhão apontando para a direita (flip espelha)
        for (int x = 10; x <= 15; x++) Set(x, 2, gray);

        // Corpo
        for (int y = 4; y <= 8; y++)
            for (int x = 0; x <= 15; x++) Set(x, y, baseColor);
        for (int x = 0; x <= 15; x++) Set(x, 4, light);

        // Esteira animada (o offset alterna os elos, simulando rotação)
        for (int y = 9; y <= 11; y++)
            for (int x = 0; x <= 15; x++)
                Set(x, y, (x + trackOffset) % 2 == 0 ? Color.Black * 0.85f : dark);

        return MakeTexture(w, h, px);
    }

    // ------------------------------------------------------------------
    // BLOB (batedor): 12x12 px, criatura redonda com pezinhos
    // ------------------------------------------------------------------
    public Texture2D[] CreateBlobFrames(Color baseColor)
    {
        return new[] { CreateBlob(baseColor, 0), CreateBlob(baseColor, 1), CreateBlob(baseColor, 2) };
    }

    private Texture2D CreateBlob(Color baseColor, int frame)
    {
        const int w = 12, h = 12;
        var px = new Color[w * h];

        var dark = Darken(baseColor, 0.55f);
        var light = Color.Lerp(baseColor, Color.White, 0.5f);

        void Set(int x, int y, Color c)
        {
            if (x >= 0 && x < w && y >= 0 && y < h) px[y * w + x] = c;
        }

        // Corpo redondo
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 12; x++)
            {
                var dx = x - 5.5f;
                var dy = y - 4.5f;
                if (dx * dx + dy * dy <= 22f)
                    Set(x, y, dx + dy < -3.5f ? light : baseColor);
            }
        }

        // Olhos
        Set(6, 4, Color.White); Set(8, 4, Color.White);
        Set(6, 5, Color.Black); Set(8, 5, Color.Black);

        // Pezinhos (animam com a corrida)
        int leftFoot = frame == 1 ? 2 : frame == 2 ? 4 : 3;
        int rightFoot = frame == 1 ? 9 : frame == 2 ? 7 : 8;
        Set(leftFoot, 10, dark); Set(leftFoot, 11, dark);
        Set(rightFoot, 10, dark); Set(rightFoot, 11, dark);

        return MakeTexture(w, h, px);
    }

    // ------------------------------------------------------------------
    // TERRENO: tile 64x64 com grama, flores e pedras
    // ------------------------------------------------------------------
    public Texture2D CreateTerrainTile(int size = 64)
    {
        var px = new Color[size * size];
        var grassBase = new Color(58, 68, 48);
        var grassDark = new Color(48, 58, 38);
        var grassLight = new Color(68, 78, 58);
        var dirt = new Color(62, 52, 42);

        // Base de grama com variação
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var roll = _random.NextDouble();
                var noise = Math.Sin(x * 0.3) * Math.Cos(y * 0.3) * 0.1;
                
                px[y * size + x] = roll < 0.05 + noise ? grassLight
                                 : roll < 0.15 ? grassDark
                                 : roll < 0.18 ? dirt
                                 : grassBase;
            }
        }

        // Pequenas flores coloridas ocasionais (2x2 pixels)
        if (_random.NextDouble() < 0.4)
        {
            var fx = _random.Next(5, size - 6);
            var fy = _random.Next(5, size - 6);
            var flowerColor = _random.Next(3) switch
            {
                0 => new Color(255, 220, 80),  // amarela
                1 => new Color(255, 150, 180), // rosa
                _ => new Color(200, 180, 255)  // roxa
            };
            px[fy * size + fx] = flowerColor;
            px[fy * size + fx + 1] = flowerColor * 0.8f;
        }

        // Pedrinhas (3x2 pixels)
        if (_random.NextDouble() < 0.3)
        {
            var rx = _random.Next(2, size - 4);
            var ry = _random.Next(2, size - 3);
            var rock = new Color(95, 90, 85);
            var rockDark = new Color(70, 68, 65);
            px[ry * size + rx] = rockDark;
            px[ry * size + rx + 1] = rock;
            px[ry * size + rx + 2] = rockDark;
            px[(ry + 1) * size + rx] = rock;
            px[(ry + 1) * size + rx + 1] = rock;
            px[(ry + 1) * size + rx + 2] = rockDark;
        }

        return MakeTexture(size, size, px);
    }

    // ------------------------------------------------------------------
    // SOMBRA: elipse suave com gradiente
    // ------------------------------------------------------------------
    public Texture2D CreateShadow(int w = 32, int h = 12)
    {
        var px = new Color[w * h];
        var cx = (w - 1) / 2f;
        var cy = (h - 1) / 2f;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var nx = (x - cx) / cx;
                var ny = (y - cy) / cy;
                var dist = nx * nx + ny * ny;
                
                if (dist <= 1f)
                {
                    var alpha = (1f - dist) * 0.45f; // gradiente suave
                    px[y * w + x] = new Color(0, 0, 0, alpha);
                }
            }
        }

        return MakeTexture(w, h, px);
    }
    
    // ------------------------------------------------------------------
    // PARTÍCULA: pequeno quadrado para efeitos
    // ------------------------------------------------------------------
    public Texture2D CreateParticle(int size = 3)
    {
        var px = new Color[size * size];
        var center = size / 2;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var dx = x - center;
                var dy = y - center;
                var dist = (float)Math.Sqrt(dx * dx + dy * dy);
                var alpha = 1f - Math.Min(1f, dist / center);
                px[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        
        return MakeTexture(size, size, px);
    }

    private Texture2D MakeTexture(int w, int h, Color[] pixels)
    {
        var texture = new Texture2D(_device, w, h);
        texture.SetData(pixels);
        return texture;
    }

    private static Color Darken(Color color, float factor)
    {
        return new Color((int)(color.R * factor), (int)(color.G * factor), (int)(color.B * factor));
    }
}
