using UnityEngine;

/// <summary>
/// 可收集触发器。
/// 玩家接触后通知管理器并销毁自身。
/// </summary>
public class CollectibleTrigger : MonoBehaviour
{
    [Header("References")]
    public MultiTriggerManager manager; // 关联的管理器

    [Header("Audio")]
    public string collectSFX = "deskInteract_GoldCoin"; // 拾取时的音效 ID

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=cyan>[CollectibleDebug]</color> {gameObject.name} 被物体撞击: {other.name}");

        if (triggered) return;

        // 检查是否为玩家 (根据项目约定 type == EntityType.Player)
        EntityCore otherCore = other.GetComponent<EntityCore>();
        if (otherCore != null && otherCore.type == EntityType.Player)
        {
            triggered = true;
            Debug.Log($"<color=green>[CollectibleDebug]</color> 确认撞击物为玩家！通知管理器...");
            
            // 播放音效
            if (AudioManager.Instance != null && !string.IsNullOrEmpty(collectSFX))
            {
                AudioManager.Instance.PlaySFX(collectSFX);
            }

            if (manager != null)
            {
                manager.OnItemTriggered();
            }
            else
            {
                Debug.LogWarning($"<color=red>[CollectibleDebug]</color> [{gameObject.name}] 没有关联 MultiTriggerManager！");
            }

            Debug.Log($"[{gameObject.name}] 被玩家触发并消失。");
            
            // 物体消失
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"<color=orange>[CollectibleDebug]</color> 撞击物不是玩家。EntityCore存在: {otherCore != null}, 类型: {(otherCore != null ? otherCore.type.ToString() : "无")}");
        }
    }
}
