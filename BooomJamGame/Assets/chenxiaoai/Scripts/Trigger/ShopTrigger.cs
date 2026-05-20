using UnityEngine;

/// <summary>
/// 商店触发器脚本，挂载在场景中的商店物体上。
/// 当玩家卡牌接触时，呼出商店界面。
/// </summary>
public class ShopTrigger : MonoBehaviour
{
    [Header("Shop Configuration")]
    [Tooltip("起始价格")]
    public int startPrice = 20;
    [Tooltip("每次购买增加的价格")]
    public int priceIncrement = 2;
    [Header("Bonus Values")]
    [Tooltip("增加的攻击力")]
    public int attackBonus = 2;
    [Tooltip("增加的防御力")]
    public int defenseBonus = 2;
    [Tooltip("增加的生命值")]
    public int healthBonus = 10;

    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        EntityCore otherCore = other.GetComponent<EntityCore>();
        if (otherCore != null && otherCore.type == EntityType.Player)
        {
            playerInside = true;
            if (ShopManager.instance != null)
            {
                // 将当前触发器配置的数值传递给商店管理器
                ShopManager.instance.InitShopValues(startPrice, priceIncrement, attackBonus, defenseBonus, healthBonus);
                // 标记商店已解锁（永久）
                ShopManager.instance.SetNearShop(true);
                
                // 激活 UIManager 上的商店按钮
                if (UIManager.instance != null)
                {
                    UIManager.instance.EnableShopButton();
                }

                Debug.Log("[ShopTrigger] 玩家首次接触商店，已永久激活商店功能和 UI 按钮。");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 按照您的要求，碰到之后就是激活状态了，所以离开时不执行注销逻辑
        EntityCore otherCore = other.GetComponent<EntityCore>();
        if (otherCore != null && otherCore.type == EntityType.Player)
        {
            playerInside = false;
        }
    }
}
