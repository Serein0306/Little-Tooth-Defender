using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 故事 3 · Healthy Food Choice：点健康食物（苹果、胡萝卜）加分并收集；
/// 点不健康食物（糖果、蛋糕）给出提示但不加分。两个健康食物都选到即通关。
/// </summary>
public static class TDF_Story3
{
    static bool[] chosen; // [0]=苹果 [1]=胡萝卜
    static Text colRef;

    public static void Build(TDF_GameManager gm)
    {
        var page = gm.Page;
        TDF_NavBar.Build(gm, "Choose healthy food to strengthen the tooth!");
        chosen = new bool[2];

        // Collection 实时计数（覆盖在原画 Collection 附近）
        var col = TDF_UIFactory.Text("Collection", page.rootRT, "Collection  \u25CB" + gm.healthyCount + "/2", 34,
            TDF_UIFactory.DarkBlue, TextAnchor.MiddleLeft, FontStyle.Bold);
        col.rectTransform.sizeDelta = new Vector2(340, 56);
        col.rectTransform.anchoredPosition = page.ImgToAnchoredPos(new Rect(0.28f, 0.15f, 0.30f, 0.05f));
        TDF_UIFactory.AddOutline(col, Color.white, 2);
        colRef = col;

        TDF_UIFactory.InvisibleButton("Apple", page.rootRT, page.SizeFor(TDF_Zones.Story3Apple), page.ImgToAnchoredPos(TDF_Zones.Story3Apple), () => Pick(gm, true, 0));
        TDF_UIFactory.InvisibleButton("Carrot", page.rootRT, page.SizeFor(TDF_Zones.Story3Carrot), page.ImgToAnchoredPos(TDF_Zones.Story3Carrot), () => Pick(gm, true, 1));
        TDF_UIFactory.InvisibleButton("Candy", page.rootRT, page.SizeFor(TDF_Zones.Story3Candy), page.ImgToAnchoredPos(TDF_Zones.Story3Candy), () => Pick(gm, false, 0));
        TDF_UIFactory.InvisibleButton("Cake", page.rootRT, page.SizeFor(TDF_Zones.Story3Cake), page.ImgToAnchoredPos(TDF_Zones.Story3Cake), () => Pick(gm, false, 1));

        // 只高亮健康食物：苹果黄色光晕、胡萝卜橙色光晕（不健康食物不高亮，形成对比）
        TDF_Highlight.Pulse(gm, "Apple", TDF_Zones.Story3Apple, TDF_UIFactory.BrightYellow, 0f);
        TDF_Highlight.Pulse(gm, "Carrot", TDF_Zones.Story3Carrot, TDF_UIFactory.LightOrange, 0.9f);
    }

    static void Pick(TDF_GameManager gm, bool healthy, int idx)
    {
        var page = gm.Page;
        if (healthy)
        {
            if (chosen[idx]) return;
            chosen[idx] = true;
            gm.AddHealthy();
            TDF_Audio.Sparkle();
            TDF_Highlight.Hide(idx == 0 ? "Apple" : "Carrot"); // 选中的食物高亮收起

            Rect zone = idx == 0 ? TDF_Zones.Story3Apple : TDF_Zones.Story3Carrot;
            Rect r = page.ImgToAnchoredRect(zone);
            TDF_UIFactory.SpawnSticker(page.rootRT, TDF_SpriteFactory.Glow(TDF_UIFactory.BrightYellow), r.center, new Vector2(r.width * 2.2f, r.height * 2.2f));
            TDF_UIFactory.SpawnSticker(page.rootRT, TDF_SpriteFactory.Sparkle(TDF_UIFactory.BrightPink), r.center, new Vector2(r.width * 0.8f, r.width * 0.8f));

            if (colRef != null) colRef.text = "Collection  \u25CB" + gm.healthyCount + "/2";
            if (chosen[0] && chosen[1]) gm.StartCoroutine(Complete(gm));
        }
        else
        {
            TDF_Audio.Sad();
            gm.Toast(idx == 0 ? "Candy is bad for teeth!" : "Cake is bad for teeth!");

            Rect zone = idx == 0 ? TDF_Zones.Story3Candy : TDF_Zones.Story3Cake;
            Rect r = page.ImgToAnchoredRect(zone);
            TDF_UIFactory.SpawnSticker(page.rootRT, TDF_SpriteFactory.Pop(TDF_UIFactory.OrangeRed), r.center, new Vector2(r.width * 1.3f, r.height * 1.3f));
        }
    }

    static IEnumerator Complete(TDF_GameManager gm)
    {
        yield return new WaitForSeconds(0.5f);
        TDF_Audio.Success();
        gm.CompleteStory(2);
        TDF_Panels.SuccessPanel(gm, "Story 3 Complete!", "Healthy food makes a strong tooth!\nHealthy Smile Badge +1", "Next Story", TDFPage.Story4);
    }
}
