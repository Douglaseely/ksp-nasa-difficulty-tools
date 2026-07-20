#if KSP_RUNTIME
using UnityEngine;

namespace KspMissionPlanner.Plugin;

public static class MissionPlannerToolbarIconFactory
{
    public static Texture2D Create()
    {
        const int size = 38;
        var texture = new Texture2D(size, size, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
        };

        var dark = new Color(0.08f, 0.18f, 0.33f, 1f);
        var mid = new Color(0.11f, 0.36f, 0.63f, 1f);
        var accent = new Color(0.98f, 0.84f, 0.20f, 1f);
        var white = new Color(0.92f, 0.96f, 1f, 1f);

        for (var y = 0; y < size; y++)
        {
            var t = y / (float)(size - 1);
            var rowColor = Color.Lerp(dark, mid, t);
            for (var x = 0; x < size; x++)
            {
                texture.SetPixel(x, y, rowColor);
            }
        }

        DrawCircle(texture, 19, 19, 13f, white, 1.5f);
        DrawCircle(texture, 19, 19, 8f, new Color(0.75f, 0.88f, 1f, 1f), 1.3f);
        FillCircle(texture, 25, 11, 2.8f, accent);
        FillCircle(texture, 13, 25, 2.2f, accent);
        DrawLine(texture, 13, 25, 25, 11, new Color(1f, 0.95f, 0.62f, 1f));

        texture.Apply();
        return texture;
    }

    private static void DrawCircle(Texture2D texture, int cx, int cy, float radius, Color color, float thickness)
    {
        var rMin = radius - thickness;
        var rMax = radius + thickness;
        var rMinSq = rMin * rMin;
        var rMaxSq = rMax * rMax;

        for (var y = 0; y < texture.height; y++)
        {
            for (var x = 0; x < texture.width; x++)
            {
                var dx = x - cx;
                var dy = y - cy;
                var d = dx * dx + dy * dy;
                if (d >= rMinSq && d <= rMaxSq)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private static void FillCircle(Texture2D texture, int cx, int cy, float radius, Color color)
    {
        var rSq = radius * radius;
        for (var y = 0; y < texture.height; y++)
        {
            for (var x = 0; x < texture.width; x++)
            {
                var dx = x - cx;
                var dy = y - cy;
                if (dx * dx + dy * dy <= rSq)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, Color color)
    {
        var dx = Mathf.Abs(x1 - x0);
        var sx = x0 < x1 ? 1 : -1;
        var dy = -Mathf.Abs(y1 - y0);
        var sy = y0 < y1 ? 1 : -1;
        var err = dx + dy;
        var x = x0;
        var y = y0;

        while (true)
        {
            if (x >= 0 && y >= 0 && x < texture.width && y < texture.height)
            {
                texture.SetPixel(x, y, color);
            }

            if (x == x1 && y == y1)
            {
                break;
            }

            var e2 = 2 * err;
            if (e2 >= dy)
            {
                err += dy;
                x += sx;
            }

            if (e2 <= dx)
            {
                err += dx;
                y += sy;
            }
        }
    }
}
#endif