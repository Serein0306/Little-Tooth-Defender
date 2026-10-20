using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 一页完整的页面：整张原画完整显示、永不裁切（contain，居中），
/// 并用与原画底色接近的填充色铺满留白区域，使留白"隐形"。
/// 任何窗口比例下：全部内容可见、没有刺眼的白边、热区始终与原画对齐。
/// </summary>
public class TDF_Page
{
    public string name;
    public GameObject root;
    public RectTransform rootRT;
    public Vector2 dispSize;   // 原画实际显示尺寸（画布单位）
    public Vector2 dispOffset; // 原画左下角相对画布左下角的偏移（画布单位）
    public Vector2 canvasSize;

    public static TDF_Page Create(Transform canvas, string pageName, Texture2D tex, Vector2 canvasSize)
    {
        var p = new TDF_Page();
        p.name = pageName;
        p.canvasSize = canvasSize;
        p.root = new GameObject("Page_" + pageName, typeof(RectTransform));
        p.rootRT = (RectTransform)p.root.transform;
        p.rootRT.SetParent(canvas, false);
        TDF_UIFactory.Stretch(p.rootRT);

        // 完整显示（contain）：图片较宽 → 按宽铺满、上下留白；图片较高 → 按高铺满、左右留白。
        // 注意：宽高比用"原画真实比例"计算，不读纹理导入尺寸（部分导入设置会把图片裁成正方形）。
        float ia = AspectFor(pageName);
        float ca = canvasSize.x / canvasSize.y;
        float dispW, dispH;
        if (ia >= ca)
        {
            dispW = canvasSize.x; dispH = dispW / ia;
        }
        else
        {
            dispH = canvasSize.y; dispW = dispH * ia;
        }

        p.dispSize = new Vector2(dispW, dispH);
        p.dispOffset = new Vector2((dispW - canvasSize.x) * 0.5f, (dispH - canvasSize.y) * 0.5f);

        // 底层：与原画底色接近的填充色，铺满整个画布（让留白区域不刺眼）
        var fillGo = new GameObject("BG_Fill", typeof(RectTransform), typeof(Image));
        fillGo.transform.SetParent(p.rootRT, false);
        var fillRT = (RectTransform)fillGo.transform;
        TDF_UIFactory.Stretch(fillRT);
        var fillImg = fillGo.GetComponent<Image>();
        fillImg.sprite = TDF_SpriteFactory.White();
        fillImg.color = BgFor(pageName);
        fillImg.raycastTarget = false;

        // 上层：原画本身，按原比例居中显示
        var bgGo = new GameObject("BG", typeof(RectTransform), typeof(RawImage));
        bgGo.transform.SetParent(p.rootRT, false);
        var bgRT = (RectTransform)bgGo.transform;
        bgRT.sizeDelta = p.dispSize;
        bgRT.anchoredPosition = new Vector2(p.dispOffset.x + dispW * 0.5f - canvasSize.x * 0.5f,
                                            p.dispOffset.y + dispH * 0.5f - canvasSize.y * 0.5f);
        var img = bgGo.GetComponent<RawImage>();
        img.texture = tex;
        img.raycastTarget = false;
        return p;
    }

    /// 每页原画的真实宽高比（宽/高），取自已提供的原画文件，避免依赖纹理导入尺寸。
    static float AspectFor(string pageName)
    {
        switch (pageName)
        {
            case "cover":   return 1536f / 2730f;  // 启动封面
            case "home":    return 816f / 1460f;  // 新版主页
            case "story1":  return 952f / 1680f;  // 糖虫攻击
            case "story2":  return 667f / 1175f;  // 刷牙大战
            case "story3":  return 997f / 1764f;  // 健康食物
            case "story4":  return 960f / 1770f;  // 牙线救援
            case "credits": return 910f / 1621f;  // 致谢页
            default: return 0.5625f;
        }
    }

    /// 每页的留白填充色（与原画底色接近，取自各页画面主色）
    static Color BgFor(string pageName)
    {
        switch (pageName)
        {
            case "cover":  return new Color(0.93f, 0.87f, 0.95f); // 淡粉紫（封面渐变背景）
            case "home":   return new Color(0.84f, 0.78f, 0.90f); // 淡紫（主页天空/笔触）
            case "story3": return new Color(0.97f, 0.87f, 0.91f); // 淡粉（健康食物页背景）
            case "credits":return new Color(0.98f, 0.86f, 0.86f); // 淡粉（致谢页）
            default:       return new Color(1.00f, 1.00f, 1.00f); // 白（故事1/2/4 白色底纹）
        }
    }

    /// 原图归一化矩形 → 画布锚点矩形（原点在画布中心）。
    /// 热区坐标约定：y 从图片底部算起（0=底、1=顶），直接线性换算，无需翻转。
    public Rect ImgToAnchoredRect(Rect img)
    {
        float x = dispOffset.x + img.x * dispSize.x - canvasSize.x * 0.5f;
        float y = dispOffset.y + img.y * dispSize.y - canvasSize.y * 0.5f;
        return new Rect(x, y, img.width * dispSize.x, img.height * dispSize.y);
    }

    public Vector2 ImgToAnchoredPos(Rect img)
    {
        return ImgToAnchoredRect(img).center;
    }

    public Vector2 SizeFor(Rect img)
    {
        return new Vector2(img.width * dispSize.x, img.height * dispSize.y);
    }
}
