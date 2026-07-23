using Microsoft.Xna.Framework;

namespace ModularGameEngine.Engine.Util;

/// <summary>
/// Utilitários de cor compartilhados entre Engine e Game.
/// </summary>
public static class ColorUtil
{
    public static Color ParseHex(string hexColor, Color? fallback = null)
    {
        try
        {
            hexColor = hexColor.TrimStart('#');
            if (hexColor.Length < 6)
                return fallback ?? Color.White;

            var r = Convert.ToByte(hexColor.Substring(0, 2), 16);
            var g = Convert.ToByte(hexColor.Substring(2, 2), 16);
            var b = Convert.ToByte(hexColor.Substring(4, 2), 16);
            return new Color(r, g, b);
        }
        catch
        {
            return fallback ?? Color.White;
        }
    }
}
