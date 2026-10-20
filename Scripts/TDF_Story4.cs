using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 故事 4 · Floss to the Rescue：把牙线拖到右侧 Boss 糖虫上持续接触，
/// Boss 逐渐缩小，血量归零后通关，集齐 4 枚 Healthy Smile 徽章。
/// </summary>
public static class TDF_Story4
{
    const int BOSS_MAX_HP = 4;
    const float CONTACT_SECONDS = 0.35f; // 接触多久造成一次伤害

    static int bossHp;
    static bool defeated;
    static float contactTime;
    static Rect bossRect;
    static RectTransform flossRT;
    static Vector2 lastFlossPos;
    static TDF_Page pageRef;
    static TDF_GameManager gmRef;
    static bool watchRunning;

    public static void Build(TDF_GameManager gm)
    {
        var page = gm.Page;
        pageRef = page;
        gmRef = gm;
        TDF_NavBar.Build(gm, "Use the floss to reach the boss sugar bug!");

        bossHp = BOSS_MAX_HP;
        defeated = false;
        contactTime = 0f;
        bossRect = page.ImgToAnchoredRect(TDF_Zones.Story4Boss);

        // 徽章进度（覆盖在原画 badges 附近）
        var b = TDF_UIFactory.Text("Badges", page.rootRT, "Healthy Smile x" + gm.CompletedCount(), 34,
            TDF_UIFactory.DarkBlue, TextAnchor.MiddleLeft, FontStyle.Bold);
        b.rectTransform.sizeDelta = new Vector2(420, 54);
        b.rectTransform.anchoredPosition = page.ImgToAnchoredPos(new Rect(0.55f, 0.22f, 0.43f, 0.05f));
        TDF_UIFactory.AddOutline(b, Color.white, 2);

        // 可拖拽的牙线
        Vector2 startPos = page.ImgToAnchoredPos(TDF_Zones.Story4FlossHome);
        lastFlossPos = startPos;
        var flossImg = TDF_UIFactory.SpawnSticker(page.rootRT, TDF_SpriteFactory.Floss(), startPos, new Vector2(340, 46), true);
        flossRT = flossImg.rectTransform;
        var drag = flossImg.gameObject.AddComponent<TDF_Draggable>();
        drag.canvasRT = gm.CanvasRT;
        drag.onDrag = OnFlossDrag;

        // 持续检测牙线是否停在 Boss 上
        if (!watchRunning) { watchRunning = true; gm.StartCoroutine(WatchFloss()); }

        // 操作高亮：牙线薄荷绿光晕（引导拖拽起点）、Boss 紫色光晕（引导目标）
        TDF_Highlight.Pulse(gm, "Floss", TDF_Zones.Story4FlossHome, TDF_UIFactory.Mint, 0f);
        TDF_Highlight.Pulse(gm, "Boss", TDF_Zones.Story4Boss, TDF_UIFactory.BrightPurple, 1.2f);
    }

    static void OnFlossDrag(Vector2 pos)
    {
        TDF_Highlight.HideAll(); // 玩家开始拖牙线，提示全部收起
        if (defeated || flossRT == null) return;
        lastFlossPos = pos;

        // 让牙线朝向 Boss，拖动更有"瞄准"的感觉
        Vector2 dir = bossRect.center - pos;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        flossRT.rotation = Quaternion.Euler(0f, 0f, ang);
    }

    static IEnumerator WatchFloss()
    {
        while (!defeated && pageRef != null && pageRef.root != null && gmRef != null && gmRef.Current == TDFPage.Story4)
        {
            if (bossRect.Contains(lastFlossPos))
            {
                contactTime += Time.deltaTime;
                if (contactTime >= CONTACT_SECONDS)
                {
                    contactTime = 0f;
                    DamageBoss();
                }
            }
            else contactTime = 0f;
            yield return null;
        }
        watchRunning = false;
    }

    static void DamageBoss()
    {
        bossHp--;
        TDF_Audio.Shrink();

        // 用逐渐变小的绿色圆形覆盖 Boss，表现"缩小"
        float size = 300f * (bossHp + 1) / (BOSS_MAX_HP + 1);
        TDF_UIFactory.SpawnSticker(pageRef.rootRT, TDF_SpriteFactory.Circle(new Color(0.30f, 0.72f, 0.30f, 1f), 90), bossRect.center, new Vector2(size, size));

        if (bossHp <= 0)
        {
            defeated = true;
            TDF_Audio.Win();
            gmRef.CompleteStory(3);
            gmRef.StartCoroutine(Finish());
        }
    }

    static IEnumerator Finish()
    {
        yield return new WaitForSeconds(0.8f);
        TDF_Panels.SuccessPanel(gmRef, "Story 4 Complete!", "Boss sugar bug defeated!\nHealthy Smile Badges: x" + gmRef.CompletedCount(), "Credits", TDFPage.Credits);
    }
}
