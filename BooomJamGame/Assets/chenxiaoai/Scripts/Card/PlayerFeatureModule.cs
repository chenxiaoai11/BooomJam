using UnityEngine;

/// <summary>
/// 玩家功能模块，处理商店和技能树的解锁状态同步。
/// 继承自 ModuleBase，符合项目模块化规范。
/// </summary>
public class PlayerFeatureModule : ModuleBase
{
    public override void OnModuleLoad(EntityCore entity)
    {
        // 加载时，如果 GameManager 已经有数据，会自动由 PlayerPersistenceModule 同步。
        // 这里我们可以根据需要进行额外的初始化。
        if (UIManager.instance != null)
        {
            UIManager.instance.RefreshButtonStates();
        }
    }

    public override void OnModuleTick()
    {
        // 可以在这里处理一些快捷键，如果需要的话
        if (Input.GetKeyDown(KeyCode.B))
        {
            // 商店快捷键：如果 UIManager 上的商店按钮是激活的（说明本关已踩过商店），则允许打开
            if (UIManager.instance != null && UIManager.instance.shopButton != null && UIManager.instance.shopButton.activeSelf)
            {
                UIManager.instance.OnShopButtonClick();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.K)) // 假设 K 是技能树快捷键
        {
            if (GameManager.instance != null && GameManager.instance.isSkillTreeUnlocked)
            {
                if (UIManager.instance != null) UIManager.instance.OnSkillTreeButtonClick();
            }
        }
    }

    public override void OnModuleUnload()
    {
        // 卸载逻辑
    }

    /// <summary>
    /// 解锁商店功能（仅限本关卡）
    /// </summary>
    public void UnlockShop()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.EnableShopButton();
        }
    }

    /// <summary>
    /// 解锁技能树功能
    /// </summary>
    public void UnlockSkillTree()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.isSkillTreeUnlocked = true;
            if (UIManager.instance != null)
            {
                UIManager.instance.EnableSkillTreeButton();
            }
        }
    }
}
