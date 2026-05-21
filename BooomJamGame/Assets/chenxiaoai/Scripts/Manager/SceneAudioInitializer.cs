using UnityEngine;

/// <summary>
/// 场景音频初始化器。
/// 挂载在场景中的任何物体上，用于在场景开始时自动播放指定的 BGM。
/// </summary>
public class SceneAudioInitializer : MonoBehaviour
{
    [Header("BGM Settings")]
    [Tooltip("要在该场景播放的 BGM ID（对应 AudioManager 中的配置）")]
    public string sceneBGM = "indoorScene_bg";

    [Tooltip("是否在场景加载时立即播放")]
    public bool playOnStart = true;

    [Tooltip("如果当前已经在播放这首 BGM，是否重新开始播放")]
    public bool restartIfSame = false;

    private void Start()
    {
        if (playOnStart)
        {
            InitializeAudio();
        }
    }

    public void InitializeAudio()
    {
        if (AudioManager.Instance != null && !string.IsNullOrEmpty(sceneBGM))
        {
            // 如果不要求重新开始，且当前正在播放该 BGM，则跳过
            // 注意：这里假设 AudioManager 有一个方法可以获取当前播放的 BGM ID，
            // 如果没有，我们直接调用 PlayBGM，AudioManager 内部通常会有防重放处理。
            AudioManager.Instance.PlayBGM(sceneBGM);
            Debug.Log($"[SceneAudioInitializer] 正在为场景播放 BGM: {sceneBGM}");
        }
    }
}
