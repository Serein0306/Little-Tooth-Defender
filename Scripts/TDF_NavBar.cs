using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 故事页顶部的导航栏（Back / Read / Volume / Progress），
/// 透明按钮精确覆盖在原画自带的导航按钮上。
/// </summary>
public static class TDF_NavBar
{
    public static void Build(TDF_GameManager gm, string narration)
    {
        var page = gm.Page;
        TDF_UIFactory.InvisibleButton("Nav_Back", page.rootRT, page.SizeFor(TDF_Zones.NavBack), page.ImgToAnchoredPos(TDF_Zones.NavBack), () => gm.Back());
        TDF_UIFactory.InvisibleButton("Nav_Read", page.rootRT, page.SizeFor(TDF_Zones.NavRead), page.ImgToAnchoredPos(TDF_Zones.NavRead), () => TDF_Panels.ReadOverlay(gm, narration));
        TDF_UIFactory.InvisibleButton("Nav_Volume", page.rootRT, page.SizeFor(TDF_Zones.NavVolume), page.ImgToAnchoredPos(TDF_Zones.NavVolume), () => gm.ToggleMute());
        TDF_UIFactory.InvisibleButton("Nav_Progress", page.rootRT, page.SizeFor(TDF_Zones.NavProgress), page.ImgToAnchoredPos(TDF_Zones.NavProgress), () => TDF_Panels.ProgressPanel(gm));

        // 音量状态小标签（跟随音量按钮下方）
        Vector2 volPos = page.ImgToAnchoredPos(TDF_Zones.NavVolume);
        var label = TDF_UIFactory.Text("VolState", page.rootRT, TDF_Audio.Muted ? "OFF" : "ON", 22, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        label.rectTransform.sizeDelta = new Vector2(110, 34);
        label.rectTransform.anchoredPosition = new Vector2(volPos.x, volPos.y - 55);
        TDF_UIFactory.AddOutline(label, Color.black, 2);
    }
}
