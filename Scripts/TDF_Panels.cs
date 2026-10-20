using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 各类弹层：进度面板、设置面板、朗读面板、通关弹窗、Toast 提示。
/// </summary>
public static class TDF_Panels
{
    static GameObject top;

    public static GameObject Top(TDF_GameManager gm)
    {
        CloseTop();
        top = new GameObject("Panel", typeof(RectTransform));
        top.transform.SetParent(gm.CanvasRT, false);
        TDF_UIFactory.Stretch((RectTransform)top.transform);
        TDF_UIFactory.Dim(top.transform, Color.black, 0.35f);
        return top;
    }

    public static void CloseTop()
    {
        if (top != null) { Object.Destroy(top); top = null; }
    }

    public static void Toast(TDF_GameManager gm, string msg)
    {
        var rt = TDF_UIFactory.Rect("Toast", gm.CanvasRT);
        rt.anchoredPosition = new Vector2(0f, -gm.CanvasSize.y * 0.30f);
        var img = rt.gameObject.AddComponent<Image>();
        img.sprite = TDF_SpriteFactory.White();
        img.color = new Color(0.12f, 0.14f, 0.20f, 0.78f);
        rt.sizeDelta = new Vector2(gm.CanvasSize.x * 0.92f, 96);
        var t = TDF_UIFactory.Text("Msg", rt, msg, 38, Color.white);
        TDF_UIFactory.Stretch(t.rectTransform);
        rt.SetAsLastSibling();
        gm.StartCoroutine(FadeOut(rt));
    }

    static IEnumerator FadeOut(RectTransform rt)
    {
        yield return new WaitForSeconds(1.8f);
        if (rt != null) Object.Destroy(rt.gameObject);
    }

    public static void SuccessPanel(TDF_GameManager gm, string title, string subtitle, string nextLabel, TDFPage nextPage)
    {
        var root = Top(gm);
        var panel = TDF_UIFactory.Rect("Success", root.transform);
        panel.sizeDelta = new Vector2(gm.CanvasSize.x * 0.80f, gm.CanvasSize.y * 0.32f);
        panel.anchoredPosition = Vector2.zero;
        var pimg = panel.gameObject.AddComponent<Image>();
        pimg.sprite = TDF_SpriteFactory.White();
        pimg.color = new Color(1f, 1f, 1f, 0.97f);

        var titleT = TDF_UIFactory.Text("Title", panel, title, 54, TDF_UIFactory.DarkBlue, TextAnchor.MiddleCenter, FontStyle.Bold);
        titleT.rectTransform.sizeDelta = new Vector2(panel.sizeDelta.x - 40, 90);
        titleT.rectTransform.anchoredPosition = new Vector2(0f, 90f);

        var subT = TDF_UIFactory.Text("Sub", panel, subtitle, 34, TDF_UIFactory.Brown);
        subT.rectTransform.sizeDelta = new Vector2(panel.sizeDelta.x - 60, 120);
        subT.rectTransform.anchoredPosition = new Vector2(0f, -10f);

        TDF_UIFactory.Button("Next", panel, TDF_UIFactory.BrightYellow, nextLabel, 36, TDF_UIFactory.DarkBlue,
            new Vector2(300, 78), new Vector2(-130, -140), () => { CloseTop(); gm.Go(nextPage); });
        TDF_UIFactory.Button("Home", panel, TDF_UIFactory.Mint, "Home", 32, Color.white,
            new Vector2(210, 66), new Vector2(180, -140), () => { CloseTop(); gm.Go(TDFPage.Home); });
    }

    public static void ReadOverlay(TDF_GameManager gm, string narration)
    {
        var root = Top(gm);
        var panel = TDF_UIFactory.Rect("Read", root.transform);
        panel.anchoredPosition = new Vector2(0f, -gm.CanvasSize.y * 0.20f);
        panel.sizeDelta = new Vector2(gm.CanvasSize.x * 0.86f, 250);
        var pimg = panel.gameObject.AddComponent<Image>();
        pimg.sprite = TDF_SpriteFactory.White();
        pimg.color = new Color(1f, 1f, 0.95f, 0.97f);

        var t = TDF_UIFactory.Text("Narration", panel, "", 40, TDF_UIFactory.DarkBlue);
        t.rectTransform.sizeDelta = new Vector2(panel.sizeDelta.x - 60, 170);
        t.rectTransform.anchoredPosition = new Vector2(0f, 10f);
        gm.StartCoroutine(Typewriter(t, narration));

        // 点面板外任意处关闭
        TDF_UIFactory.InvisibleButton("Close", root.transform, gm.CanvasSize, Vector2.zero, () => CloseTop());
        panel.SetAsLastSibling();
        TDF_UIFactory.Button("OK", panel, TDF_UIFactory.BrightYellow, "OK", 32, TDF_UIFactory.DarkBlue,
            new Vector2(180, 62), new Vector2(0f, -105), () => CloseTop());
    }

    static IEnumerator Typewriter(Text t, string full)
    {
        t.text = "";
        foreach (char c in full)
        {
            t.text += c;
            if (char.IsLetterOrDigit(c)) TDF_Audio.Tick();
            yield return new WaitForSeconds(0.03f);
        }
    }

    public static void ProgressPanel(TDF_GameManager gm)
    {
        var root = Top(gm);
        var panel = TDF_UIFactory.Rect("Progress", root.transform);
        panel.sizeDelta = new Vector2(gm.CanvasSize.x * 0.84f, gm.CanvasSize.y * 0.56f);
        panel.anchoredPosition = Vector2.zero;
        var pimg = panel.gameObject.AddComponent<Image>();
        pimg.sprite = TDF_SpriteFactory.White();
        pimg.color = new Color(1f, 1f, 1f, 0.97f);

        var titleT = TDF_UIFactory.Text("Title", panel, "Little Tooth Defender", 46, TDF_UIFactory.DarkBlue, TextAnchor.MiddleCenter, FontStyle.Bold);
        titleT.rectTransform.sizeDelta = new Vector2(panel.sizeDelta.x - 40, 80);
        titleT.rectTransform.anchoredPosition = new Vector2(0f, 190f);

        string[] names =
        {
            "Story 1 - Sugar Bugs Attack",
            "Story 2 - Brushing Battle",
            "Story 3 - Healthy Food Choice",
            "Story 4 - Floss to the Rescue"
        };
        for (int i = 0; i < 4; i++)
        {
            float y = 70 - i * 118;
            var rowT = TDF_UIFactory.Text("Row" + i, panel, names[i], 34, TDF_UIFactory.Brown, TextAnchor.MiddleLeft);
            rowT.rectTransform.anchoredPosition = new Vector2(-250, y);
            rowT.rectTransform.sizeDelta = new Vector2(620, 60);

            var check = TDF_UIFactory.Text("Check" + i, panel, gm.IsCompleted(i) ? "\u2713" : "\u25CB", 40,
                gm.IsCompleted(i) ? TDF_UIFactory.Mint : TDF_UIFactory.LightGray, TextAnchor.MiddleCenter, FontStyle.Bold);
            check.rectTransform.anchoredPosition = new Vector2(290, y);
            check.rectTransform.sizeDelta = new Vector2(80, 60);
        }

        var badgesT = TDF_UIFactory.Text("Badges", panel, "Healthy Smile Badges: " + gm.CompletedCount() + " / 4", 34,
            TDF_UIFactory.BrightPink, TextAnchor.MiddleCenter, FontStyle.Bold);
        badgesT.rectTransform.sizeDelta = new Vector2(panel.sizeDelta.x - 40, 60);
        badgesT.rectTransform.anchoredPosition = new Vector2(0f, -390);

        TDF_UIFactory.Button("Reset", panel, TDF_UIFactory.OrangeRed, "Reset", 30, Color.white,
            new Vector2(220, 72), new Vector2(-150, -490), () => { gm.ResetProgress(); CloseTop(); gm.Toast("Progress reset!"); });
        TDF_UIFactory.Button("Close", panel, TDF_UIFactory.BrightYellow, "Close", 30, TDF_UIFactory.DarkBlue,
            new Vector2(180, 72), new Vector2(150, -490), () => CloseTop());
    }

    public static void SettingsPanel(TDF_GameManager gm)
    {
        var root = Top(gm);
        var panel = TDF_UIFactory.Rect("Settings", root.transform);
        panel.sizeDelta = new Vector2(gm.CanvasSize.x * 0.72f, gm.CanvasSize.y * 0.42f);
        panel.anchoredPosition = Vector2.zero;
        var pimg = panel.gameObject.AddComponent<Image>();
        pimg.sprite = TDF_SpriteFactory.White();
        pimg.color = new Color(1f, 1f, 1f, 0.97f);

        var titleT = TDF_UIFactory.Text("Title", panel, "Settings", 46, TDF_UIFactory.DarkBlue, TextAnchor.MiddleCenter, FontStyle.Bold);
        titleT.rectTransform.sizeDelta = new Vector2(panel.sizeDelta.x - 40, 80);
        titleT.rectTransform.anchoredPosition = new Vector2(0f, 130f);

        var soundT = TDF_UIFactory.Text("Sound", panel, TDF_Audio.Muted ? "Sound: OFF" : "Sound: ON", 34, TDF_UIFactory.Brown);
        soundT.rectTransform.anchoredPosition = new Vector2(-60, 0);
        soundT.rectTransform.sizeDelta = new Vector2(420, 60);

        TDF_UIFactory.Button("Toggle", panel, TDF_UIFactory.Mint, "Toggle", 28, Color.white,
            new Vector2(180, 64), new Vector2(280, 0), () => { gm.ToggleMute(); soundT.text = TDF_Audio.Muted ? "Sound: OFF" : "Sound: ON"; });
        TDF_UIFactory.Button("Reset", panel, TDF_UIFactory.OrangeRed, "Reset Progress", 28, Color.white,
            new Vector2(280, 64), new Vector2(0, -110), () => { gm.ResetProgress(); CloseTop(); gm.Toast("Progress reset!"); });
        TDF_UIFactory.Button("Close", panel, TDF_UIFactory.BrightYellow, "Close", 28, TDF_UIFactory.DarkBlue,
            new Vector2(180, 64), new Vector2(0, -210), () => CloseTop());
    }
}
