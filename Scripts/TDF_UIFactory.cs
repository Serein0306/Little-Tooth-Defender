using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI 工厂：统一创建 Canvas、图片、文字、按钮，配色取自上传的色卡。
/// </summary>
public static class TDF_UIFactory
{
    // ---- 色卡（来自上传的 Color Palette 原图）----
    public static readonly Color Red         = Hex(0xFF001C);
    public static readonly Color Rose        = Hex(0xFF005F);
    public static readonly Color DarkBlue    = Hex(0x035BA0);
    public static readonly Color SkyBlue     = Hex(0x1E8ED2);
    public static readonly Color Mint        = Hex(0x1ED2B3);
    public static readonly Color LightGray   = Hex(0xDEDEDE);
    public static readonly Color BrightYellow = Hex(0xFFD400);
    public static readonly Color BrightPurple = Hex(0xD800FF);
    public static readonly Color BrightPink   = Hex(0xFF00CE);
    public static readonly Color OrangeRed    = Hex(0xD74F19);
    public static readonly Color LightOrange  = Hex(0xEC6722);
    public static readonly Color Brown        = Hex(0x7B340F);
    public static readonly Color NearWhite    = Hex(0xE9E9E9);

    static Color Hex(uint v)
    {
        return new Color(((v >> 16) & 0xFF) / 255f, ((v >> 8) & 0xFF) / 255f, (v & 0xFF) / 255f, 1f);
    }

    static Font _font;
    public static Font Font()
    {
        if (_font != null) return _font;
        // 新版 Unity 内置字体已改名为 LegacyRuntime.ttf，旧版仍用 Arial.ttf，
        // 逐个尝试并吞掉异常，保证任何版本都能取到字体。
        _font = TryLoad("LegacyRuntime.ttf");
        if (_font == null) _font = TryLoad("Arial.ttf");
        return _font;
    }

    static Font TryLoad(string name)
    {
        try { return Resources.GetBuiltinResource<Font>(name); }
        catch { return null; }
    }

    public static Canvas BuildCanvas()
    {
        var go = new GameObject("Canvas", typeof(RectTransform));
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    public static RectTransform Rect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    public static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public static Image Image(string name, Transform parent, Color color, Vector2 size, Vector2 anchoredPos)
    {
        var rt = Rect(name, parent);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        var img = rt.gameObject.AddComponent<Image>();
        img.sprite = TDF_SpriteFactory.White();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    public static RawImage RawImage(string name, Transform parent, Texture tex)
    {
        var rt = Rect(name, parent);
        Stretch(rt);
        var img = rt.gameObject.AddComponent<RawImage>();
        img.texture = tex;
        img.raycastTarget = false;
        return img;
    }

    public static Text Text(string name, Transform parent, string content, int size, Color color,
        TextAnchor align = TextAnchor.MiddleCenter, FontStyle style = FontStyle.Normal)
    {
        var rt = Rect(name, parent);
        var t = rt.gameObject.AddComponent<Text>();
        t.font = Font();
        t.text = content;
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.fontStyle = style;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }

    public static void AddOutline(Text t, Color color, float size = 2f)
    {
        var o = t.gameObject.AddComponent<Outline>();
        o.effectColor = color;
        o.effectDistance = new Vector2(size, -size);
    }

    /// 实色按钮（带悬停/按下反馈与点击音效）
    public static Button Button(string name, Transform parent, Color bg, string label, int fontSize, Color labelColor,
        Vector2 size, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
    {
        var rt = Rect(name, parent);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        var img = rt.gameObject.AddComponent<Image>();
        img.sprite = TDF_SpriteFactory.White();
        img.color = bg;
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        var cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        cb.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
        btn.colors = cb;
        btn.onClick.AddListener(onClick);
        btn.onClick.AddListener(() => TDF_Audio.Click());
        var t = Text("Label", rt, label, fontSize, labelColor);
        Stretch(t.rectTransform);
        return btn;
    }

    /// 覆盖在原画按钮上的隐形按钮（只接收点击，不遮挡画面）
    public static Button InvisibleButton(string name, Transform parent, Vector2 size, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
    {
        var rt = Rect(name, parent);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        var img = rt.gameObject.AddComponent<Image>();
        img.sprite = TDF_SpriteFactory.White();
        img.color = TDF_Zones.DEBUG ? new Color(0.95f, 0.1f, 0.1f, 0.30f) : new Color(1f, 1f, 1f, 0f);
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        var cb = new ColorBlock();
        cb.normalColor = Color.white;
        cb.highlightedColor = Color.white;
        cb.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        cb.disabledColor = Color.white;
        cb.colorMultiplier = 1f;
        cb.fadeDuration = 0.05f;
        btn.colors = cb;
        btn.onClick.AddListener(onClick);
        btn.onClick.AddListener(() => TDF_Audio.Click());
        return btn;
    }

    public static void Dim(Transform parent, Color color, float alpha)
    {
        var rt = Rect("Dim", parent);
        Stretch(rt);
        var img = rt.gameObject.AddComponent<Image>();
        img.sprite = TDF_SpriteFactory.White();
        img.color = new Color(color.r, color.g, color.b, alpha);
        img.raycastTarget = true;
    }

    /// 在页面上临时显示一个贴纸（星爆、光晕等）
    public static Image SpawnSticker(Transform parent, Sprite sprite, Vector2 pos, Vector2 size, bool raycast = false)
    {
        var rt = Rect("Sticker", parent);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
        var img = rt.gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.color = Color.white;
        img.raycastTarget = raycast;
        return img;
    }
}
