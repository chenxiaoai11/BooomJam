using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum GameMode
    {
        GamePlay,
        DialogueMoment
    }
    public GameMode gameMode;
    private PlayableDirector currentPlayableDirector;
    private double currentClipEndTime;

    [Header("Persistence Data")]
    public PlayerPersistentData savedPlayerData;
    public bool hasSavedData = false;

    [System.Serializable]
    public struct PlayerPersistentData
    {
        public int defaultHealth;
        public int maxHealth;
        public int currentHealth;
        public int attack;
        public int defense;
        public int gold;
        public int skillPoints; // 新增技能点存储
        public List<string> unlockedSkills; // 新增已解锁技能列表
        public bool isSkillTreeUnlocked; // 新增：技能树是否已解锁
    }

    public void SavePlayerData(EntityCore player)
    {
        if (player == null) return;
        
        savedPlayerData.defaultHealth = player.defaultHealth;
        savedPlayerData.maxHealth = player.maxHealth;
        savedPlayerData.currentHealth = player.currentHealth;
        savedPlayerData.attack = player.attack;
        savedPlayerData.defense = player.defense;
        savedPlayerData.gold = player.gold;
        savedPlayerData.skillPoints = skillPoints; // 保存当前技能点
        savedPlayerData.unlockedSkills = new List<string>(unlockedSkillIDs); // 保存已解锁技能
        savedPlayerData.isSkillTreeUnlocked = isSkillTreeUnlocked;
        
        hasSavedData = true;
        Debug.Log("[GameManager] Player data saved.");
    }

    public void LoadPlayerData(EntityCore player)
    {
        if (player == null || !hasSavedData) return;

        player.defaultHealth = savedPlayerData.defaultHealth > 0 ? savedPlayerData.defaultHealth : player.defaultHealth;
        player.currentHealth = savedPlayerData.currentHealth;
        player.attack = savedPlayerData.attack;
        player.defense = savedPlayerData.defense;
        player.gold = savedPlayerData.gold;
        
        this.skillPoints = savedPlayerData.skillPoints; // 加载技能点
        this.unlockedSkillIDs = new List<string>(savedPlayerData.unlockedSkills); // 加载已解锁技能
        this.isSkillTreeUnlocked = savedPlayerData.isSkillTreeUnlocked;

        Debug.Log("[GameManager] Player data loaded.");

        // 数据加载后通知 UIManager 刷新按钮
        if (UIManager.instance != null)
        {
            UIManager.instance.RefreshButtonStates();
        }
    }

    [Header("Feature Unlock Status")]
    public bool isSkillTreeUnlocked = true; // 技能树默认一直开启

    [Header("Skill Tree Data")]
    public int skillPoints = 0;
    public List<string> unlockedSkillIDs = new List<string>();

    public bool IsSkillUnlocked(string skillID)
    {
        return unlockedSkillIDs.Contains(skillID);
    }

    public void UnlockSkill(string skillID)
    {
        if (!unlockedSkillIDs.Contains(skillID))
        {
            unlockedSkillIDs.Add(skillID);
        }
    }

    /// <summary>
    /// 当玩家死亡时调用：重置数据并返回主界面
    /// </summary>
    public void OnPlayerDeath()
    {
        Debug.Log("[GameManager] Player has died. Resetting game and returning to home.");
        ResetGameData();
        
        if (SceneTransitionManager.instance != null)
        {
            SceneTransitionManager.instance.TransitionToScene("Home");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        }
    }

    /// <summary>
    /// 重置所有持久化数据为初始值
    /// </summary>
    public void ResetGameData()
    {
        hasSavedData = false;
        isSkillTreeUnlocked = true; // 保持技能树始终开启的设定
        skillPoints = 0;
        unlockedSkillIDs.Clear();
        
        savedPlayerData = new PlayerPersistentData
        {
            defaultHealth = 100,
            maxHealth = 100,
            currentHealth = 100,
            attack = 10,
            defense = 5,
            gold = 10,
            skillPoints = 0,
            unlockedSkills = new List<string>(),
            isSkillTreeUnlocked = true
        };

        Debug.Log("[GameManager] All persistent data has been reset.");
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        gameMode = GameMode.GamePlay;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameMode == GameMode.DialogueMoment)
            {
                if (UIManager.instance.IsTyping)
                {
                    UIManager.instance.CompleteTypewriter();
                }
                else
                {
                    ResumeTimeline();
                }
            }
            else if (gameMode == GameMode.GamePlay && UIManager.instance != null && UIManager.instance.IsTyping)
            {
                // 如果正在播放片段时按下空格
                UIManager.instance.CompleteTypewriter();
                // 将 Timeline 时间跳转到当前片段的末尾，触发暂停
                if (currentPlayableDirector != null)
                {
                    currentPlayableDirector.time = currentClipEndTime;
                }
            }
        }
    }

    public void OnDialogueClipStart(PlayableDirector director, double endTime)
    {
        currentPlayableDirector = director;
        currentClipEndTime = endTime;
    }

    public void PauseTimeline(PlayableDirector _playableDirector)
    {
        currentPlayableDirector = _playableDirector;
        gameMode = GameMode.DialogueMoment;
        
        // 只有在 Graph 有效时才设置速度
        if (currentPlayableDirector != null && currentPlayableDirector.playableGraph.IsValid())
        {
            currentPlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        }

        if (UIManager.instance != null) UIManager.instance.ToggleSpaceBar(true);
    }

    public void ResumeTimeline()
    {
        if (currentPlayableDirector == null) return;

        // 检查是否已经到达或接近 Timeline 的终点
        if (currentPlayableDirector.time >= currentPlayableDirector.duration - 0.1f)
        {
            EndTimeline();
            return;
        }

        gameMode = GameMode.GamePlay;
        
        if (currentPlayableDirector.playableGraph.IsValid())
        {
            currentPlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.ToggleSpaceBar(false);
            UIManager.instance.ToggleDialogueBox(true);
        }
    }

    public void EndTimeline()
    {
        Debug.Log("[GameManager] EndTimeline 被调用了！");
        gameMode = GameMode.GamePlay;
        
        if (UIManager.instance != null)
        {
            Debug.Log("[GameManager] 正在调用 UIManager 关闭对话框...");
            UIManager.instance.ToggleSpaceBar(false);
            UIManager.instance.ToggleDialogueBox(false);
        }
        else
        {
            Debug.LogError("[GameManager] UIManager.instance 为空，无法关闭 UI！");
        }

        if (currentPlayableDirector != null)
        {
            currentPlayableDirector.Stop();
        }
    }
}
