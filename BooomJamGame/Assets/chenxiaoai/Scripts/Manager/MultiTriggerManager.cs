using UnityEngine;
using System.Collections;

/// <summary>
/// 多重触发管理器。
/// 当指定数量的物体被收集/触发后，激活目标物体（如箭头）。
/// </summary>
public class MultiTriggerManager : MonoBehaviour
{
    [Header("Settings")]
    public int requiredCount = 2; // 需要触发的数量
    public GameObject targetToActivate; // 触发完成后要激活的物体（可选）
    
    [Header("Level Transition")]
    public bool transitionToNextScene = false; // 是否在完成后跳转关卡
    public string targetSceneName = ""; // 新增：跳转的目标场景名称，若为空则跳转到 Build Settings 的下一个
    public float transitionDelay = 1.0f; // 跳转前的延迟时间
    public string completeSFX = "deskScene_Win"; // 全部完成后播放的音效 ID

    [Header("Debug Info")]
    [SerializeField] private int currentCount = 0; // 改为序列化以方便在 Inspector 查看进度

    private void Awake()
    {
        // 初始确保目标物体是隐藏的
        if (targetToActivate != null)
        {
            targetToActivate.SetActive(false);
        }
    }

    /// <summary>
    /// 当一个子项被触发时调用
    /// </summary>
    public void OnItemTriggered()
    {
        currentCount++;
        Debug.Log($"<color=yellow>[MultiTriggerManager]</color> 触发成功！当前进度: {currentCount}/{requiredCount}");

        if (currentCount >= requiredCount)
        {
            Debug.Log("<color=green>[MultiTriggerManager]</color> 达成全部触发条件！");
            
            // 播放完成音效
            if (AudioManager.Instance != null && !string.IsNullOrEmpty(completeSFX))
            {
                AudioManager.Instance.PlaySFX(completeSFX);
            }

            if (targetToActivate != null) ActivateTarget();
            
            if (transitionToNextScene)
            {
                Debug.Log($"[MultiTriggerManager] 将在 {transitionDelay} 秒后跳转场景...");
                StartCoroutine(WaitAndTransition());
            }
        }
    }

    private IEnumerator WaitAndTransition()
    {
        yield return new WaitForSeconds(transitionDelay);
        DoTransition();
    }

    private void ActivateTarget()
    {
        targetToActivate.SetActive(true);
        Debug.Log("[MultiTriggerManager] 所有条件已达成，目标物体已激活！");
    }

    private void DoTransition()
    {
        Debug.Log("[MultiTriggerManager] 正在准备跳转到场景...");
        if (SceneTransitionManager.instance != null)
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneTransitionManager.instance.TransitionToScene(targetSceneName);
            }
            else
            {
                SceneTransitionManager.instance.TransitionToNextScene();
            }
        }
        else
        {
            Debug.LogError("[MultiTriggerManager] 找不到 SceneTransitionManager 实例，无法跳转关卡！");
        }
    }
}
