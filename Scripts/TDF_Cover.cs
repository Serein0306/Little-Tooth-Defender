using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 启动封面页：整页可点，点击任意位置进入主页。
/// 封面原画已含标题与 "Tap to Start" 按钮，这里只加一个全屏透明点击层。
/// 封面动画由 TDF_CoverAnim 负责（浮动 + 呼吸 + 点击音效与淡出）。
/// </summary>
public static class TDF_Cover
{
    public static void Build(TDF_GameManager gm)
    {
        var page = gm.Page;

        // 封面动画组件（挂在页面根上，随页面一起销毁）
        var anim = page.root.AddComponent<TDF_CoverAnim>();
        anim.Init(gm);

        // 全屏透明点击层：点击任意处进入主页（带音效与淡出）
        var tapGo = new GameObject("Cover_Tap", typeof(RectTransform), typeof(Image));
        tapGo.transform.SetParent(page.rootRT, false);
        var tapRT = (RectTransform)tapGo.transform;
        TDF_UIFactory.Stretch(tapRT);
        var tapImg = tapGo.GetComponent<Image>();
        tapImg.sprite = TDF_SpriteFactory.White();
        tapImg.color = new Color(1f, 1f, 1f, 0f);   // 透明
        var tapBtn = tapGo.AddComponent<Button>();
        tapBtn.transition = Selectable.Transition.None;
        tapBtn.onClick.AddListener(() => anim.Enter());
    }
}
