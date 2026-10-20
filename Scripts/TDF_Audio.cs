using UnityEngine;

/// <summary>
/// 运行时程序化合成所有音效（无需任何音频文件），符合原设计 "Sound: Self-created"。
/// 音量开关会保存到 PlayerPrefs。
/// </summary>
public static class TDF_Audio
{
    const string MUTE_KEY = "TDF_muted";

    static AudioSource src;
    static bool muted;

    public static bool Muted { get { return muted; } }

    public static void Init()
    {
        if (src != null) return;
        var go = new GameObject("TDF_Audio");
        Object.DontDestroyOnLoad(go);
        src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        muted = PlayerPrefs.GetInt(MUTE_KEY, 0) == 1;
        Apply();
    }

    public static void ToggleMute()
    {
        muted = !muted;
        PlayerPrefs.SetInt(MUTE_KEY, muted ? 1 : 0);
        PlayerPrefs.Save();
        Apply();
    }

    static void Apply() { AudioListener.volume = muted ? 0f : 1f; }

    // ---- 各反馈音效 ----
    public static void Click()   { Play(Tone(700, 900, 0.05f, 0.35f, Wave.Square)); }
    public static void Tick()    { Play(Tone(1200, 1200, 0.03f, 0.15f, Wave.Sine)); }
    public static void Pop()     { Play(Tone(520, 150, 0.14f, 0.5f, Wave.Square)); Play(Noise(0.05f, 0.25f)); }
    public static void Scrub()   { Play(Noise(0.12f, 0.35f)); }
    public static void Sparkle() { Play(Mix(Tone(660, 660, 0.09f, 0.3f, Wave.Sine), 0f,
                                            Tone(880, 880, 0.09f, 0.3f, Wave.Sine), 0.09f,
                                            Tone(1320, 1320, 0.14f, 0.3f, Wave.Sine), 0.18f)); }
    public static void Success() { Play(Mix(Tone(523, 523, 0.12f, 0.35f, Wave.Sine), 0f,
                                            Tone(659, 659, 0.12f, 0.35f, Wave.Sine), 0.12f,
                                            Tone(784, 784, 0.12f, 0.35f, Wave.Sine), 0.24f,
                                            Tone(1046, 1046, 0.30f, 0.40f, Wave.Sine), 0.36f)); }
    public static void Sad()     { Play(Mix(Tone(392, 392, 0.15f, 0.30f, Wave.Sine), 0f,
                                            Tone(330, 330, 0.15f, 0.30f, Wave.Sine), 0.15f,
                                            Tone(262, 262, 0.30f, 0.30f, Wave.Sine), 0.30f)); }
    public static void Shrink()  { Play(Tone(880, 180, 0.40f, 0.35f, Wave.Sine)); }
    public static void Win()     { Success(); Sparkle(); }

    enum Wave { Sine, Square }

    static int Sr { get { return AudioSettings.outputSampleRate; } }

    static float[] Tone(float f0, float f1, float dur, float vol, Wave wave)
    {
        int n = Mathf.Max(1, Mathf.CeilToInt(dur * Sr));
        var data = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / Sr;
            float f = Mathf.Lerp(f0, f1, t / dur);
            float ph = Mathf.Repeat(f * t, 1f);
            float v = wave == Wave.Sine ? Mathf.Sin(ph * Mathf.PI * 2f) : (ph < 0.5f ? 1f : -1f);
            float env = Mathf.Sin(Mathf.PI * ((float)i / n));
            data[i] = v * env * vol * 0.35f;
        }
        return data;
    }

    static float[] Noise(float dur, float vol)
    {
        int n = Mathf.Max(1, Mathf.CeilToInt(dur * Sr));
        var data = new float[n];
        for (int i = 0; i < n; i++)
        {
            float env = Mathf.Sin(Mathf.PI * ((float)i / n));
            data[i] = (Random.value * 2f - 1f) * env * vol * 0.30f;
        }
        return data;
    }

    /// 将多段音频按起始时间叠加（参数：clip, start, clip, start, ...）
    static float[] Mix(params object[] parts)
    {
        float dur = 0f;
        for (int i = 0; i < parts.Length; i += 2)
        {
            var d = (float[])parts[i];
            float start = (float)parts[i + 1];
            dur = Mathf.Max(dur, start + d.Length / (float)Sr);
        }
        var data = new float[Mathf.CeilToInt(dur * Sr)];
        for (int i = 0; i < parts.Length; i += 2)
        {
            var d = (float[])parts[i];
            int startIdx = Mathf.FloorToInt((float)parts[i + 1] * Sr);
            for (int j = 0; j < d.Length && startIdx + j < data.Length; j++)
                data[startIdx + j] += d[j];
        }
        return data;
    }

    static void Play(float[] samples)
    {
        if (muted || src == null) return;
        var clip = AudioClip.Create("sfx", samples.Length, 1, Sr, false);
        clip.SetData(samples, 0);
        src.PlayOneShot(clip, 1f);
    }
}
