using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 故事 2 · Brushing Battle：拖动牙刷擦掉牙齿上的糖虫，全部擦净后点 Rinse 完成。
/// </summary>
public static class TDF_Story2
{
    const int HITS_NEEDED = 3; // 每只糖虫需要擦到的次数

    static int[] hits;
    static bool[] scrubbed;
    static Rect[] bugRects;
    static TDF_Page pageRef;
    static bool rinseDone;

    public static void Build(TDF_GameManager gm)
    {
        var page = gm.Page;
        pageRef = page;
        TDF_NavBar.Build(gm, "Drag the toothbrush to scrub the sugar bugs away!");

        int n = TDF_Zones.Story2Bugs.Length;
        hits = new int[n];
        scrubbed = new bool[n];
        bugRects = new Rect[n];
        for (int i = 0; i < n; i++) bugRects[i] = page.ImgToAnchoredRect(TDF_Zones.Story2Bugs[i]);
        rinseDone = false;

        // 原画上的 Brush / Rinse 按钮
        TDF_UIFactory.InvisibleButton("BrushBtn", page.rootRT, page.SizeFor(TDF_Zones.Story2Brush), page.ImgToAnchoredPos(TDF_Zones.Story2Brush),
            () => gm.Toast("Scrub the sugar bugs with the brush!"));
        TDF_UIFactory.InvisibleButton("RinseBtn", page.rootRT, page.SizeFor(TDF_Zones.Story2Rinse), page.ImgToAnchoredPos(TDF_Zones.Story2Rinse),
            () => RinseAction(gm));

        // 可拖拽的牙刷
        Vector2 brushPos = page.ImgToAnchoredPos(TDF_Zones.Story2BrushHome);
        var brushImg = TDF_UIFactory.SpawnSticker(page.rootRT, TDF_SpriteFactory.Toothbrush(), brushPos, new Vector2(230, 106), true);
        var drag = brushImg.gameObject.AddComponent<TDF_Draggable>();
        drag.canvasRT = gm.CanvasRT;
        drag.onDrag = OnBrushDrag;

        // 操作高亮：牙刷黄色光晕 + 3 只糖虫薄荷绿光晕（相位错开更生动）
        TDF_Highlight.Pulse(gm, "Brush", TDF_Zones.Story2BrushHome, TDF_UIFactory.BrightYellow, 0f);
        for (int i = 0; i < TDF_Zones.Story2Bugs.Length; i++)
            TDF_Highlight.Pulse(gm, "Bug" + i, TDF_Zones.Story2Bugs[i], TDF_UIFactory.Mint, 0.9f + i * 0.8f);
    }

    static void OnBrushDrag(Vector2 pos)
    {
        TDF_Highlight.HideAll(); // 玩家开始拖动牙刷，提示全部收起
        for (int i = 0; i < bugRects.Length; i++)
        {
            if (scrubbed[i]) continue;
            if (bugRects[i].Contains(pos))
            {
                hits[i]++;
                TDF_Audio.Scrub();
                if (hits[i] >= HITS_NEEDED)
                {
                    scrubbed[i] = true;
                    TDF_Audio.Pop();
                    Rect r = pageRef.ImgToAnchoredRect(TDF_Zones.Story2Bugs[i]);
                    TDF_UIFactory.SpawnSticker(pageRef.rootRT, TDF_SpriteFactory.Pop(TDF_UIFactory.Mint), r.center, new Vector2(r.width * 1.2f, r.height * 1.2f));
                }
            }
        }
    }

    static void RinseAction(TDF_GameManager gm)
    {
        bool all = true;
        for (int i = 0; i < scrubbed.Length; i++) all &= scrubbed[i];
        if (!all) { TDF_Audio.Sad(); gm.Toast("Scrub all the sugar bugs first!"); return; }
        if (rinseDone) return;
        rinseDone = true;
        gm.StartCoroutine(Complete(gm));
    }

    static IEnumerator Complete(TDF_GameManager gm)
    {
        yield return new WaitForSeconds(0.4f);
        TDF_Audio.Success();
        gm.CompleteStory(1);
        TDF_Panels.SuccessPanel(gm, "Story 2 Complete!", "Teeth are sparkling clean!\nHealthy Smile Badge +1", "Next Story", TDFPage.Story3);
    }
}
