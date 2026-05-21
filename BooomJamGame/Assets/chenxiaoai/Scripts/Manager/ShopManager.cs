using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// 商店管理器，控制商店 UI 的显示、隐藏以及购买逻辑。
/// </summary>
public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("UI Components")]
    public RectTransform shopPanel;
    public UnityEngine.UI.Image blockerMask; // 重新加入遮罩引用
    public Button buyAttackBtn;
    public Button buyDefenseBtn;
    public Button buyHealthBtn;
    public Button exitBtn;
    public TextMeshProUGUI priceBannerText;

    [Header("Settings (Current)")]
    public float slideDuration = 0.5f;
    public KeyCode shopShortcutKey = KeyCode.B; // 默认使用 B 键打开商店
    public int itemPrice = 20;
    public int priceIncrement = 2;
    public int attackBonus = 2;
    public int defenseBonus = 2;
    public int healthBonus = 10;
    
    public Vector2 hiddenPos = new Vector2(0, 1000);
    public Vector2 visiblePos = Vector2.zero;

    private bool isNearShop = false;
    private bool isShopOpen = false;

    private void Awake()
    {
        instance = this;
        
        // 初始隐藏并禁用面板物体
        if (shopPanel != null)
        {
            shopPanel.anchoredPosition = hiddenPos;
            shopPanel.gameObject.SetActive(false);
        }

        // 初始关闭遮罩
        if (blockerMask != null) blockerMask.enabled = false;

        // 绑定按钮事件
        if (buyAttackBtn != null) buyAttackBtn.onClick.AddListener(() => Purchase("Attack"));
        if (buyDefenseBtn != null) buyDefenseBtn.onClick.AddListener(() => Purchase("Defense"));
        if (buyHealthBtn != null) buyHealthBtn.onClick.AddListener(() => Purchase("Health"));
        if (exitBtn != null) exitBtn.onClick.AddListener(CloseShop);

        UpdatePriceUI();
    }

    private void Start()
    {
        // 移除：不再每关自动根据 GameManager 恢复 nearShop 状态
        // 必须在本关内碰到 ShopTrigger 才会激活
    }

    private void Update()
    {
        // 如果玩家在商店附近，且按下了快捷键
        if (isNearShop && Input.GetKeyDown(shopShortcutKey))
        {
            if (isShopOpen)
                CloseShop();
            else
                OpenShop();
        }
    }

    /// <summary>
    /// 设置玩家是否在商店附近（已修改为永久激活逻辑）
    /// </summary>
    public void SetNearShop(bool state)
    {
        // 如果已经永久激活，则不再改变状态
        if (isNearShop) return;

        isNearShop = state;
        
        if (state)
        {
            // 碰到商店，激活面板物体，但保持在隐藏位置
            if (shopPanel != null) shopPanel.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 初始化商店数值（由触发器调用）
    /// </summary>
    public void InitShopValues(int startPrice, int increment, int atkPlus, int defPlus, int hpPlus)
    {
        itemPrice = startPrice;
        priceIncrement = increment;
        attackBonus = atkPlus;
        defenseBonus = defPlus;
        healthBonus = hpPlus;
        UpdatePriceUI();
    }

    private void UpdatePriceUI()
    {
        if (priceBannerText != null) priceBannerText.text = "全场 " + itemPrice;
    }

    public void OpenShop()
    {
        if (isShopOpen) return;
        isShopOpen = true;

        // 确保面板物体是激活的
        if (shopPanel != null) shopPanel.gameObject.SetActive(true);

        // 开启面板时激活遮罩并拦截 3D 场景
        if (blockerMask != null) blockerMask.enabled = true;
        UIManager.IsBlocking3DScene = true;

        if (shopPanel != null)
        {
            shopPanel.DOKill(true);
            shopPanel.DOAnchorPos(visiblePos, slideDuration).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    public void CloseShop()
    {
        if (!isShopOpen) return;
        isShopOpen = false;

        if (shopPanel != null)
        {
            shopPanel.DOKill(true);
            shopPanel.DOAnchorPos(hiddenPos, slideDuration).SetEase(Ease.InBack).SetUpdate(true)
            .OnComplete(() => {
                // 面板完全退场后禁用遮罩并恢复 3D 场景
                if (blockerMask != null) blockerMask.enabled = false;
                UIManager.IsBlocking3DScene = false;

                // 如果此时玩家已经不在商店附近了，则彻底禁用面板物体
                if (!isNearShop)
                {
                    shopPanel.gameObject.SetActive(false);
                }
            });
        }
        else
        {
            if (blockerMask != null) blockerMask.enabled = false;
            UIManager.IsBlocking3DScene = false;
        }
    }

    private void Purchase(string statType)
    {
        // 查找场景中的玩家
        EntityCore player = null;
        foreach (var core in FindObjectsOfType<EntityCore>())
        {
            if (core.type == EntityType.Player)
            {
                player = core;
                break;
            }
        }

        if (player == null)
        {
            Debug.LogWarning("[Shop] 找不到玩家，无法购买！");
            return;
        }

        // 检查金钱是否足够
        if (player.gold < itemPrice)
        {
            Debug.Log("[Shop] 金币不足！需要 " + itemPrice);
            return;
        }

        // 扣钱
        player.gold -= itemPrice;

        // 加属性
        switch (statType)
        {
            case "Attack":
                player.attack += attackBonus;
                Debug.Log($"[Shop] 购买攻击力成功！当前攻击: {player.attack} (+{attackBonus})");
                break;
            case "Defense":
                player.defense += defenseBonus;
                Debug.Log($"[Shop] 购买防御力成功！当前防御: {player.defense} (+{defenseBonus})");
                break;
            case "Health":
                player.maxHealth += healthBonus;
                player.currentHealth += healthBonus;
                Debug.Log($"[Shop] 购买生命值成功！当前生命: {player.currentHealth}/{player.maxHealth} (+{healthBonus})");
                break;
        }

        // 增加价格
        itemPrice += priceIncrement;
        UpdatePriceUI();

        // 刷新 UIManager 的金钱显示
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateGoldUI();
        }
    }
}
