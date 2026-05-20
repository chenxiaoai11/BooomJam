using UnityEngine;

/// <summary>
/// 可收集触发器。
/// 玩家接触后通知管理器并销毁自身。
/// </summary>
public class CollectibleTrigger : MonoBehaviour
{
    [Header("References")]
    public MultiTriggerManager manager; // 关联的管理器

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // 检查是否为玩家 (根据项目约定 type == EntityType.Player)
        EntityCore otherCore = other.GetComponent<EntityCore>();
        if (otherCore != null && otherCore.type == EntityType.Player)
        {
            triggered = true;
            
            if (manager != null)
            {
                manager.OnItemTriggered();
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] 没有关联 MultiTriggerManager！");
            }

            Debug.Log($"[{gameObject.name}] 被玩家触发并消失。");
            
            // 物体消失
            gameObject.SetActive(false);
            // 或者彻底销毁：Destroy(gameObject);
        }
    }
}
