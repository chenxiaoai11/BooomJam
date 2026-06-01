using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 注意：所有 using 必须写在最顶部，类外面
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EntityCore : MonoBehaviour
{
    [Header("设置")]
    public bool showDebugLogs = true;

    [Header("个体数据")]
    public int id;
    public EntityType type = EntityType.Enemy;
    public string entityName;
    public int defaultHealth = 100;
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int attack = 10;
    public int defense = 5;
    public int gold = 10;
    public List<string> skills = new List<string>();

    public bool HasHealthCap => type != EntityType.Player;

    public int BaseHealthValue => type == EntityType.Player ? defaultHealth : maxHealth;

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        if (HasHealthCap)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            return;
        }

        currentHealth += amount;
    }

    public string GetHealthDisplayText()
    {
        return HasHealthCap ? $"{currentHealth}/{maxHealth}" : currentHealth.ToString();
    }

    // 存储当前激活的模块
    private Dictionary<IModuleCore, MonoBehaviour> _moduleMap = new Dictionary<IModuleCore, MonoBehaviour>();

    void Update()
    {
        // 只执行当前活着的模块
        foreach (var module in _moduleMap.Keys.ToList())
        {
            if (_moduleMap[module] != null && _moduleMap[module].enabled)
            {
                module.OnModuleTick();
            }
        }
    }

    // --- 公共 API：供模块自动调用 ---
    public void RegisterModule(IModuleCore module, MonoBehaviour mono)
    {
        if (!_moduleMap.ContainsKey(module))
        {
            _moduleMap.Add(module, mono);
            module.OnModuleLoad(this);

            if (showDebugLogs)
                Debug.Log($"[{gameObject.name}] 热插拔：装载模块 [{mono.GetType().Name}]");
        }
    }

    public void UnregisterModule(IModuleCore module, MonoBehaviour mono)
    {
        if (_moduleMap.ContainsKey(module))
        {
            module.OnModuleUnload();
            _moduleMap.Remove(module);

            if (showDebugLogs)
                Debug.Log($"[{gameObject.name}] 热插拔：卸载模块 [{mono.GetType().Name}]");
        }
    }

    // --- 辅助工具：编辑器手动扫描模块 ---
    [ContextMenu("手动扫描所有模块")]
    public void ManualScanAllModules()
    {
        var allModules = GetComponents<IModuleCore>();
        foreach (var module in allModules)
        {
            RegisterModule(module, module as MonoBehaviour);
        }
    }

    // --- 数据查找 ---
    public EnemyData FindDataById(int targetId)
    {
#if UNITY_EDITOR
        string[] guids = AssetDatabase.FindAssets("t:EnemyDataTable");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            EnemyDataTable dataTable = AssetDatabase.LoadAssetAtPath<EnemyDataTable>(path);
            if (dataTable != null)
            {
                return dataTable.enemies.Find(e => e.id == targetId);
            }
        }
#endif
        return null;
    }

    public EnemyData FindDataByName(string targetName)
    {
#if UNITY_EDITOR
        string[] guids = AssetDatabase.FindAssets("t:EnemyDataTable");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            EnemyDataTable dataTable = AssetDatabase.LoadAssetAtPath<EnemyDataTable>(path);
            if (dataTable != null)
            {
                return dataTable.enemies.Find(e => e.name == targetName);
            }
        }
#endif
        return null;
    }
}
