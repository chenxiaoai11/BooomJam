using UnityEngine;

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
    public float transitionDelay = 1.0f; // 跳转前的延迟时间

    private int currentCount = 0;

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
        Debug.Log($"[MultiTriggerManager] 进度: {currentCount}/{requiredCount}");

        if (currentCount >= requiredCount)
        {
            if (targetToActivate != null) ActivateTarget();
            if (transitionToNextScene) Invoke("DoTransition", transitionDelay);
        }
    }

    private void ActivateTarget()
    {
        targetToActivate.SetActive(true);
        Debug.Log("[MultiTriggerManager] 所有条件已达成，目标物体已激活！");
    }

    private void DoTransition()
    {
        Debug.Log("[MultiTriggerManager] 正在准备跳转到下一关...");
        if (SceneTransitionManager.instance != null)
        {
            SceneTransitionManager.instance.TransitionToNextScene();
        }
        else
        {
            Debug.LogError("[MultiTriggerManager] 找不到 SceneTransitionManager 实例，无法跳转关卡！");
        }
    }
}
