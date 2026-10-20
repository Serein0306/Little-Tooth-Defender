using UnityEngine;

/// <summary>
/// 程序化生成游戏所需的补充素材（弹出星爆、闪光、牙刷、牙线、光晕等）。
/// 这样就不需要额外图片文件，全部在运行时即时生成。
/// </summary>
public static class TDF_SpriteFactory
{
    static Sprite _white;

    public static Sprite White()
    {
        if (_white != null) return _white;
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        _white = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100f);
        return _white;
    }

    public static Sprite Circle(Color c, int radiusPx = 64)
    {
        int d = radiusPx * 2;
        var tex = new Texture2D(d, d, TextureFormat.RGBA32, false);
        float r2 = radiusPx * radiusPx;
        for (int y = 0; y < d; y++)
            for (int x = 0; x < d; x++)
            {
                float dx = x + 0.5f - radiusPx, dy = y + 0.5f - radiusPx;
                tex.SetPixel(x, y, (dx * dx + dy * dy) <= r2 ? c : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, d, d), new Vector2(0.5f, 0.5f), 100f);
    }

    /// 径向渐变光晕（用于"食物发光"反馈）
    public static Sprite Glow(Color c, int radiusPx = 128)
    {
        int d = radiusPx * 2;
        var tex = new Texture2D(d, d, TextureFormat.RGBA32, false);
        for (int y = 0; y < d; y++)
            for (int x = 0; x < d; x++)
            {
                float dx = (x + 0.5f - radiusPx) / radiusPx, dy = (y + 0.5f - radiusPx) / radiusPx;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(1f - dist);
                tex.SetPixel(x, y, new Color(c.r, c.g, c.b, a * c.a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, d, d), new Vector2(0.5f, 0.5f), 100f);
    }

    /// 四角星光（"牙齿闪亮"反馈）
    public static Sprite Sparkle(Color c, int radiusPx = 96)
    {
        int d = radiusPx * 2;
        var tex = new Texture2D(d, d, TextureFormat.RGBA32, false);
        for (int y = 0; y < d; y++)
            for (int x = 0; x < d; x++)
            {
                float nx = (x + 0.5f - radiusPx) / radiusPx;
                float ny = (y + 0.5f - radiusPx) / radiusPx;
                float r = Mathf.Sqrt(nx * nx + ny * ny);
                float star = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));
                float a = Mathf.Clamp01(1f - r) * 0.4f + Mathf.Clamp01(1f - star * 1.4f) * 0.8f;
                if (r > 1.2f) a = 0f;
                tex.SetPixel(x, y, new Color(c.r, c.g, c.b, Mathf.Clamp01(a) * c.a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, d, d), new Vector2(0.5f, 0.5f), 100f);
    }

    /// 放射星爆（"虫子被消灭"反馈）
    public static Sprite Pop(Color c, int radiusPx = 110)
    {
        int d = radiusPx * 2;
        var tex = new Texture2D(d, d, TextureFormat.RGBA32, false);
        for (int y = 0; y < d; y++)
            for (int x = 0; x < d; x++)
            {
                float nx = (x + 0.5f - radiusPx) / radiusPx;
                float ny = (y + 0.5f - radiusPx) / radiusPx;
                float r = Mathf.Sqrt(nx * nx + ny * ny);
                if (r > 1f) { tex.SetPixel(x, y, Color.clear); continue; }
                float ang = Mathf.Atan2(ny, nx);
                float rayMask = Mathf.Abs(Mathf.Sin(ang * 10)) < 0.45f ? 1f : 0.35f;
                float a = Mathf.Clamp01(1f - r) * rayMask;
                tex.SetPixel(x, y, new Color(c.r, c.g, c.b, a * c.a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, d, d), new Vector2(0.5f, 0.5f), 100f);
    }

    public static Sprite RoundedRect(Color c, int w, int h, int radius)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                tex.SetPixel(x, y, InRoundedRect(x + 0.5f, y + 0.5f, radius, radius, w - radius, h - radius, radius) ? c : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
    }

    /// 卡通牙刷：棕色手柄 + 蓝色刷头 + 白色刷毛
    public static Sprite Toothbrush()
    {
        int w = 240, h = 110;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float px = x + 0.5f, py = y + 0.5f;
                Color col = Color.clear;
                if (InRoundedRect(px, py, 8, 38, 96, 72, 16)) col = TDF_UIFactory.Brown;          // 手柄
                else if (InRoundedRect(px, py, 92, 44, 126, 66, 10)) col = TDF_UIFactory.Brown;   // 连接部
                else if (InRoundedRect(px, py, 126, 26, 232, 84, 22)) col = TDF_UIFactory.SkyBlue; // 刷头
                else if (px > 140 && px < 226)
                {
                    float cy = (Mathf.Abs(py - 32) < 2 || Mathf.Abs(py - 45) < 2 || Mathf.Abs(py - 58) < 2 || Mathf.Abs(py - 71) < 2) ? 1f : 0f;
                    if (cy > 0f) col = Color.white;
                }
                tex.SetPixel(x, y, col);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
    }

    /// 牙线：薄荷色细线
    public static Sprite Floss()
    {
        int w = 300, h = 40;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                bool inside = InRoundedRect(x + 0.5f, y + 0.5f, 6, 10, w - 6, h - 10, 10);
                tex.SetPixel(x, y, inside ? TDF_UIFactory.Mint : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
    }

    static bool InRoundedRect(float px, float py, float x1, float y1, float x2, float y2, float rad)
    {
        if (px < x1 || px > x2 || py < y1 || py > y2) return false;
        float cx = Mathf.Clamp(px, x1 + rad, x2 - rad);
        float cy = Mathf.Clamp(py, y1 + rad, y2 - rad);
        float dx = px - cx, dy = py - cy;
        return dx * dx + dy * dy <= rad * rad;
    }
}
