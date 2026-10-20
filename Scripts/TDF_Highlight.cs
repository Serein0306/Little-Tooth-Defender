using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 交互对象高亮：在可点击 / 可拖拽的物体上叠加"呼吸脉冲光晕"，
/// 让玩家一眼看出哪些东西可以操作（无文字，纯视觉提示）。
/// 用法：TDF_Highlight.Pulse(gm, id, zone, color) 开启某个区域的高亮；
///       TDF_Highlight.Hide(id) / HideAll() 在玩家开始操作后移除。
/// 页面切换时高亮随页面一起销毁，无需手动清理。
/// </summary>
public static class TDF_Highlight
{
    static Transform container;
    static int serial;

    /// <summary>在 zone（原图归一化矩形）上叠加一个呼吸光晕，id 用于之后单独隐藏。</summary>
    public static void Pulse(TDF_GameManager gm, string id, Rect zone, Color color, float phase = 0f)
    {
        var page = gm.Page;
        if (page == null || page.rootRT == null) return;

        if (container == null || container.parent != page.rootRT || !container.gameObject.activeInHierarchy)
        {
            var rt = TDF_UIFactory.Rect("Highlights", page.rootRT);
            rt.sizeDelta = Vector2.zero;
            container = rt;
        }

        Vector2 size = page.SizeFor(zone);
        var img = TDF_UIFactory.SpawnSticker(container, TDF_SpriteFactory.Glow(color, 128), page.ImgToAnchoredPos(zone), size * 1.35f);
        img.name = "Hl_" + (string.IsNullOrEmpty(id) ? ("x" + (serial++)) : id);
        var anim = img.gameObject.AddComponent<PulsingAnim>();
        anim.Init(img, color, phase);
    }

    /// <summary>隐藏指定 id 的高亮（玩家点掉了对应的对象时）。</summary>
    public static void Hide(string id)
    {
        if (container == null) return;
        string target = "Hl_" + id;
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform c = container.GetChild(i);
            if (c != null && c.name == target) Object.Destroy(c.gameObject);
        }
    }

    /// <summary>隐藏当前页所有高亮（玩家开始操作后）。</summary>
    public static void HideAll()
    {
        if (container == null) return;
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform c = container.GetChild(i);
            if (c != null) Object.Destroy(c.gameObject);
        }
    }
}

/// <summary>
/// 呼吸脉冲动画：光晕透明度 0.28~0.73 缓慢起伏 + 轻微缩放，按相位错开更生动。
/// </summary>
public class PulsingAnim : MonoBehaviour
{
    const float SPEED = 3.0f;
    Image img;
    RectTransform rt;
    Color baseColor;
    float phase;
    float t;

    public void Init(Image i, Color c, float ph)
    {
        img = i;
        baseColor = c;
        phase = ph;
        rt = (RectTransform)transform;
    }

    void Update()
    {
        t += Time.deltaTime;
        float k = (Mathf.Sin(t * SPEED + phase) + 1f) * 0.5f; // 0~1 起伏
        if (img != null)
        {
            Color c = baseColor;
            c.a = Mathf.Clamp01(0.28f + k * 0.45f);
            img.color = c;
        }
        if (rt != null)
        {
            float s = 1f + k * 0.06f;
            rt.localScale = new Vector3(s, s, 1f);
        }
    }
}
