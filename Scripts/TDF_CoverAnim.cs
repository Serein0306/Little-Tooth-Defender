using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 封面页动画：整张封面柔和上下浮动 + 呼吸缩放；
/// 点击后播放音效并淡出，随后进入主页。
/// </summary>
public class TDF_CoverAnim : MonoBehaviour
{
    RawImage bg;
    Vector2 basePos;
    TDF_GameManager gm;
    bool entered;

    public void Init(TDF_GameManager g) { gm = g; }

    void Start()
    {
        var t = transform.Find("BG");
        if (t != null) bg = t.GetComponent<RawImage>();
        if (bg != null) basePos = bg.rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (entered || bg == null || gm == null) return;
        float time = Time.time;
        // 上下浮动：幅度 ±10 画布单位，周期约 4 秒
        bg.rectTransform.anchoredPosition = basePos + new Vector2(0f, Mathf.Sin(time * 1.6f) * 10f);
        // 呼吸缩放：±1.2%，周期约 5.7 秒
        float s = 1f + Mathf.Sin(time * 1.1f) * 0.012f;
        bg.rectTransform.localScale = new Vector3(s, s, 1f);
    }

    /// 点击封面：音效 + 淡出 + 进入主页
    public void Enter()
    {
        if (entered) return;
        entered = true;
        StartCoroutine(DoEnter());
    }

    IEnumerator DoEnter()
    {
        TDF_Audio.Click();
        TDF_Audio.Sparkle();
        float dur = 0.3f, t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / dur);
            if (bg != null) bg.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }
        if (bg != null) bg.color = Color.clear;
        gm.StartGame();
    }
}
