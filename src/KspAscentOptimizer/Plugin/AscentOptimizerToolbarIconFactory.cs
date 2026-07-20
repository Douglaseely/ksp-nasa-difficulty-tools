#if KSP_RUNTIME
using UnityEngine;

namespace KspAscentOptimizer.Plugin;

public static class AscentOptimizerToolbarIconFactory
{
    public static Texture2D Create()
    {
        const int size = 38;
        var texture = new Texture2D(size, size, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
        };

        var dark = new Color(0.23f, 0.10f, 0.05f, 1f);
        var hot = new Color(0.88f, 0.39f, 0.07f, 1f);
        var ember = new Color(0.98f, 0.76f, 0.20f, 1f);
        var white = new Color(0.96f, 0.96f, 0.97f, 1f);

        for (var y = 0; y < size; y++)
        {
            var t = y / (float)(size - 1);
            var rowColor = Color.Lerp(dark, hot, t);
            for (var x = 0; x < size; x++)
            {
                texture.SetPixel(x, y, rowColor);
            }
        }

        FillTriangle(texture, new Vector2(19f, 7f), new Vector2(14f, 24f), new Vector2(24f, 24f), white);
        FillTriangle(texture, new Vector2(19f, 27f), new Vector2(16f, 33f), new Vector2(22f, 33f), ember);
        FillTriangle(texture, new Vector2(19f, 29f), new Vector2(17f, 36f), new Vector2(21f, 36f), new Color(1f, 0.92f, 0.62f, 1f));
        DrawPixelGlow(texture, 19, 5, 2, ember);

        texture.Apply();
        return texture;
    }

    private static void FillTriangle(Texture2D texture, Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        var minX = Mathf.FloorToInt(Mathf.Min(a.x, Mathf.Min(b.x, c.x)));
        var maxX = Mathf.CeilToInt(Mathf.Max(a.x, Mathf.Max(b.x, c.x)));
        var minY = Mathf.FloorToInt(Mathf.Min(a.y, Mathf.Min(b.y, c.y)));
        var maxY = Mathf.CeilToInt(Mathf.Max(a.y, Mathf.Max(b.y, c.y)));

        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                if (PointInTriangle(new Vector2(x + 0.5f, y + 0.5f), a, b, c))
                {
                    if (x >= 0 && y >= 0 && x < texture.width && y < texture.height)
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
            }
        }
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        var s1 = Sign(p, a, b);
        var s2 = Sign(p, b, c);
        var s3 = Sign(p, c, a);
        var hasNeg = s1 < 0 || s2 < 0 || s3 < 0;
        var hasPos = s1 > 0 || s2 > 0 || s3 > 0;
        return !(hasNeg && hasPos);
    }

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    private static void DrawPixelGlow(Texture2D texture, int cx, int cy, int radius, Color color)
    {
        var rSq = radius * radius;
        for (var y = cy - radius; y <= cy + radius; y++)
        {
            for (var x = cx - radius; x <= cx + radius; x++)
            {
                if (x < 0 || y < 0 || x >= texture.width || y >= texture.height)
                {
                    continue;
                }

                var dx = x - cx;
                var dy = y - cy;
                if (dx * dx + dy * dy <= rSq)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }
}
#endif