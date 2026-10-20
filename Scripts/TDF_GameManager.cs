using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TDFPage { Cover, Home, Story1, Story2, Story3, Story4, Credits }

/// <summary>
/// 游戏总控：页面切换、进度（徽章）保存、健康食物收集计数、音量与提示。
/// 所有页面 UI 都在运行时由代码构建，因此场景里只需要这一个脚本。
/// </summary>
public class TDF_GameManager : MonoBehaviour
{
    public static TDF_GameManager I;

    const string PREFS_COMPLETED = "TDF_completed";
    const string PREFS_HEALTHY = "TDF_healthy";

    Canvas canvas;
    RectTransform canvasRT;
    Vector2 canvasSize;
    readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
    GameObject currentPage;
    TDF_Page page;
    TDFPage current = TDFPage.Cover;
    int completedMask;

    public int healthyCount { get; private set; }

    void Awake()
    {
        I = this;
        TDF_Audio.Init();
        completedMask = PlayerPrefs.GetInt(PREFS_COMPLETED, 0);
        healthyCount = PlayerPrefs.GetInt(PREFS_HEALTHY, 0);
    }

    void Start()
    {
        // 运行时场景没有 AudioListener 会导致无声并刷警告，这里确保存在一个
        if (Object.FindObjectOfType<AudioListener>() == null)
            new GameObject("AudioListener").AddComponent<AudioListener>();

        TDF_Music.Play(gameObject);

        canvas = TDF_UIFactory.BuildCanvas();
        canvasRT = (RectTransform)canvas.transform;
        StartCoroutine(InitPages());
    }

    /// 等一帧再建页面，确保 CanvasScaler 已把画布实际尺寸算好（否则比例会错乱）。
    IEnumerator InitPages()
    {
        yield return null;
        Debug.Log("[TDF] Screen=" + Screen.width + "x" + Screen.height
                  + " res=" + Screen.currentResolution + " dpi=" + Screen.dpi);
        Go(TDFPage.Home);   // 播放后直接进入主页（封面仅用于编辑器未播放时的预览）
    }

    public RectTransform CanvasRT { get { return canvasRT; } }
    public Vector2 CanvasSize { get { return canvasSize; } }
    public TDF_Page Page { get { return page; } }
    public TDFPage Current { get { return current; } }

    public Texture2D LoadTexture(string key)
    {
        Texture2D t;
        if (textures.TryGetValue(key, out t) && t != null) return t;
        t = Resources.Load<Texture2D>("Sprites/page_" + key);
        if (t == null) Debug.LogError("Missing resource: Sprites/page_" + key + " (请确认 Assets/Resources/Sprites 下有对应图片)");
        textures[key] = t;
        return t;
    }

    public void Go(TDFPage target)
    {
        if (currentPage != null) Object.Destroy(currentPage);
        current = target;

        canvasSize = canvasRT != null ? canvasRT.rect.size : new Vector2(1080f, 1920f);
        if (canvasSize.x < 1f || canvasSize.y < 1f) canvasSize = new Vector2(1080f, 1920f);

        string texKey;
        switch (target)
        {
            case TDFPage.Cover: texKey = "cover"; break;
            case TDFPage.Story1: texKey = "story1"; break;
            case TDFPage.Story2: texKey = "story2"; break;
            case TDFPage.Story3: texKey = "story3"; break;
            case TDFPage.Story4: texKey = "story4"; break;
            case TDFPage.Credits: texKey = "credits"; break;
            default: texKey = "home"; break;
        }

        page = TDF_Page.Create(canvasRT, texKey, LoadTexture(texKey), canvasSize);
        Debug.Log("[TDF] Page " + texKey + " canvas=" + canvasSize
                  + " disp=" + page.dispSize + " offset=" + page.dispOffset);
        currentPage = page.root;

        switch (target)
        {
            case TDFPage.Cover: TDF_Cover.Build(this); break;
            case TDFPage.Home: TDF_Home.Build(this); break;
            case TDFPage.Story1: TDF_Story1.Build(this); break;
            case TDFPage.Story2: TDF_Story2.Build(this); break;
            case TDFPage.Story3: TDF_Story3.Build(this); break;
            case TDFPage.Story4: TDF_Story4.Build(this); break;
            case TDFPage.Credits: TDF_Credits.Build(this); break;
        }
    }

    /// 封面页点击后进入主页。
    public void StartGame() { Go(TDFPage.Home); }

    public bool IsCompleted(int storyIndex) { return (completedMask & (1 << storyIndex)) != 0; }

    public int CompletedCount()
    {
        int c = 0;
        for (int i = 0; i < 4; i++) if (IsCompleted(i)) c++;
        return c;
    }

    public void CompleteStory(int storyIndex)
    {
        if (!IsCompleted(storyIndex))
        {
            completedMask |= 1 << storyIndex;
            PlayerPrefs.SetInt(PREFS_COMPLETED, completedMask);
            PlayerPrefs.Save();
        }
    }

    public void AddHealthy()
    {
        healthyCount++;
        PlayerPrefs.SetInt(PREFS_HEALTHY, healthyCount);
        PlayerPrefs.Save();
    }

    public void ResetProgress()
    {
        completedMask = 0;
        healthyCount = 0;
        PlayerPrefs.SetInt(PREFS_COMPLETED, 0);
        PlayerPrefs.SetInt(PREFS_HEALTHY, 0);
        PlayerPrefs.Save();
    }

    public void Back() { Go(TDFPage.Home); }

    public void ContinueGame()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!IsCompleted(i)) { Go(StoryOf(i)); return; }
        }
        Go(TDFPage.Credits);
    }

    public static TDFPage StoryOf(int index)
    {
        switch (index)
        {
            case 0: return TDFPage.Story1;
            case 1: return TDFPage.Story2;
            case 2: return TDFPage.Story3;
            case 3: return TDFPage.Story4;
            default: return TDFPage.Home;
        }
    }

    public void ToggleMute()
    {
        TDF_Audio.ToggleMute();
        Toast(TDF_Audio.Muted ? "Sound OFF" : "Sound ON");
    }

    public void Toast(string msg) { TDF_Panels.Toast(this, msg); }
}
