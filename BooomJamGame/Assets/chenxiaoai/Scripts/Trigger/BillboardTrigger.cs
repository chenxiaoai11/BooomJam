using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 公告栏触发器。当玩家接触到此触发器时，调用 BillboardManager 打开指定的 UI。
/// </summary>
public class BillboardTrigger : MonoBehaviour
{
    [Header("配置")]
    [Tooltip("是否打开所有面板")]
    public bool openAllPanels = true;
    
    [Tooltip("如果不打开所有，请在此指定要打开的面板索引（对应 BillboardManager 中的 Panels 列表）")]
    public List<int> panelIndicesToOpen = new List<int> { 0 };

    private void OnTriggerEnter(Collider other)
    {
        // 如果场景正在位移切换，忽略触发
        if (ObjectSwitcherTrigger.isAnyTriggerRunning) return;

        // 1. 检查进入的物体是否有 EntityCore
        EntityCore otherCore = other.GetComponent<EntityCore>();

        // 2. 判断是否为玩家 (根据项目约定 type == EntityType.Player)
        if (otherCore != null && otherCore.type == EntityType.Player)
        {
            if (BillboardManager.instance != null)
            {
                if (openAllPanels)
                {
                    BillboardManager.instance.OpenBillboard();
                }
                else
                {
                    BillboardManager.instance.OpenPanels(panelIndicesToOpen);
                }
                
                Debug.Log($"[BillboardTrigger] 玩家接触了公告栏 [{gameObject.name}]，正在打开面板。");
            }
            else
            {
                Debug.LogWarning("[BillboardTrigger] 场景中没有找到 BillboardManager 实例！");
            }
        }
    }
}
