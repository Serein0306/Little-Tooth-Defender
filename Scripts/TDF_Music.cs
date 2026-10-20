using UnityEngine;

/// <summary>
/// 背景音乐：游戏启动后循环播放。
/// 把音乐文件放到 Assets/Resources/music.mp3，会自动加载。
/// </summary>
public static class TDF_Music
{
    static AudioSource source;

    public static void Play(GameObject host)
    {
        if (source != null) return;
        source = host.AddComponent<AudioSource>();
        source.loop = true;
        source.volume = 0.4f;

        var clip = Resources.Load<AudioClip>("music");
        if (clip != null)
        {
            source.clip = clip;
            source.Play();
        }
    }

    public static void SetVolume(float v)
    {
        if (source != null) source.volume = v;
    }

    public static void Stop()
    {
        if (source != null) source.Stop();
    }
}
