using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主页：Start / Continue / Settings（透明按钮覆盖在原画按钮上），右下角显示徽章进度。
/// </summary>
public static class TDF_Home
{
    public static void Build(TDF_GameManager gm)
    {
        var page = gm.Page;
        TDF_UIFactory.InvisibleButton("Start", page.rootRT, page.SizeFor(TDF_Zones.HomeStart), page.ImgToAnchoredPos(TDF_Zones.HomeStart), () => { TDF_Highlight.HideAll(); gm.Go(TDFPage.Story1); });
        TDF_UIFactory.InvisibleButton("Continue", page.rootRT, page.SizeFor(TDF_Zones.HomeContinue), page.ImgToAnchoredPos(TDF_Zones.HomeContinue), () => { TDF_Highlight.HideAll(); gm.ContinueGame(); });
        TDF_UIFactory.InvisibleButton("Settings", page.rootRT, page.SizeFor(TDF_Zones.HomeSettings), page.ImgToAnchoredPos(TDF_Zones.HomeSettings), () => { TDF_Highlight.HideAll(); TDF_Panels.SettingsPanel(gm); });

        var badge = TDF_UIFactory.Text("Badges", page.rootRT, "Healthy Smile x" + gm.CompletedCount(), 26, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        badge.rectTransform.sizeDelta = new Vector2(300, 44);
        badge.rectTransform.anchoredPosition = new Vector2(330, -gm.CanvasSize.y * 0.5f + 46);
        TDF_UIFactory.AddOutline(badge, Color.black, 2);

        // Home 页不显示呼吸光晕（已取消提示）
    }
}
