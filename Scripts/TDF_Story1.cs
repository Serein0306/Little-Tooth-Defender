using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 故事 1 · Sugar Bugs Attack：点按所有糖虫（点一下爆一个）。
/// </summary>
public static class TDF_Story1
{
    static bool[] popped;
    static int count;

    public static void Build(TDF_GameManager gm)
    {
        var page = gm.Page;
        TDF_NavBar.Build(gm, "Sugar bugs are attacking! Tap the sugar bugs to stop them!");

        popped = new bool[TDF_Zones.Story1Bugs.Length];
        count = 0;
        for (int i = 0; i < TDF_Zones.Story1Bugs.Length; i++)
        {
            int idx = i;
            TDF_UIFactory.InvisibleButton("Bug" + i, page.rootRT, page.SizeFor(TDF_Zones.Story1Bugs[i]), page.ImgToAnchoredPos(TDF_Zones.Story1Bugs[i]),
                () => TapBug(gm, idx));
            // 糖虫呼吸光晕已取消
        }
    }

    static void TapBug(TDF_GameManager gm, int idx)
    {
        if (popped[idx]) return;
        popped[idx] = true;
        count++;
        TDF_Highlight.Hide("Bug" + idx);

        var page = gm.Page;
        Rect r = page.ImgToAnchoredRect(TDF_Zones.Story1Bugs[idx]);
        TDF_UIFactory.SpawnSticker(page.rootRT, TDF_SpriteFactory.Pop(TDF_UIFactory.BrightYellow), r.center, new Vector2(r.width * 1.1f, r.height * 1.1f));
        TDF_Audio.Pop();
        gm.StartCoroutine(Pulse(gm, r.center));

        if (count >= popped.Length) gm.StartCoroutine(Complete(gm));
    }

    static IEnumerator Pulse(TDF_GameManager gm, Vector2 pos)
    {
        var img = TDF_UIFactory.SpawnSticker(gm.Page.rootRT, TDF_SpriteFactory.Circle(Color.white, 48), pos, new Vector2(60, 60));
        float t = 0f;
        while (t < 0.35f)
        {
            if (img == null) yield break; // 页面已切换，停止动画
            t += Time.deltaTime;
            float k = t / 0.35f;
            img.rectTransform.sizeDelta = Vector2.Lerp(new Vector2(60, 60), new Vector2(300, 300), k);
            img.color = new Color(1f, 1f, 1f, 1f - k);
            yield return null;
        }
        if (img != null) Object.Destroy(img.gameObject);
    }

    static IEnumerator Complete(TDF_GameManager gm)
    {
        yield return new WaitForSeconds(0.6f);
        TDF_Audio.Success();
        gm.CompleteStory(0);
        TDF_Panels.SuccessPanel(gm, "Story 1 Complete!", "Sugar bugs defeated!\nHealthy Smile Badge +1", "Next Story", TDFPage.Story2);
    }
}
