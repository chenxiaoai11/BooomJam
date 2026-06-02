using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 公告栏管理器，负责管理多个 BillboardPanel 的动画和显示状态。
/// 支持通过不同的 Trigger 打开不同的 Panel 组合。
/// </summary>
public class BillboardManager : MonoBehaviour
{
    public static BillboardManager instance;

    [Header("UI Panels (按顺序配置)")]
    public List<RectTransform> panels = new List<RectTransform>();

    [Header("Animation Settings")]
    [Tooltip("从左向右滑入的时间")]
    public float slideDuration = 0.5f;

    [System.Serializable]
    public class PanelPositionConfig
    {
        public Vector2 hiddenPos = new Vector2(-2000, 0);
        public Vector2 visiblePos = new Vector2(-400, 0);
    }

    [Header("Panel Positions (与 UI Panels 列表一一对应)")]
    public List<PanelPositionConfig> panelPositions = new List<PanelPositionConfig>();

    private List<bool> panelOpenStates = new List<bool>();
    private bool justOpened = false;

    private void Awake()
    {
        if (instance == null) instance = this;

        // 初始化：确保所有面板都在隐藏位置，并初始化状态列表
        panelOpenStates.Clear();
        for (int i = 0; i < panels.Count; i++)
        {
            if (panels[i] != null)
            {
                // 确保位置配置存在
                while (panelPositions.Count <= i)
                {
                    panelPositions.Add(new PanelPositionConfig());
                }

                panels[i].anchoredPosition = panelPositions[i].hiddenPos;
                panelOpenStates.Add(false);
            }
        }
    }

    private void Update()
    {
        // 如果有面板开启，且玩家点击了鼠标左键，尝试关闭面板
        if (IsAnyPanelOpen() && !justOpened && Input.GetMouseButtonDown(0))
        {
            if (!IsClickingOnAnyOpenPanel())
            {
                CloseAllPanels();
            }
        }
    }

    /// <summary>
    /// 打开指定索引的面板
    /// </summary>
    public void OpenPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= panels.Count)
        {
            Debug.LogWarning($"[BillboardManager] 尝试打开无效的面板索引: {panelIndex}");
            return;
        }

        RectTransform panel = panels[panelIndex];
        if (panel == null) return;

        // 确保列表大小匹配
        while (panelOpenStates.Count <= panelIndex)
        {
            panelOpenStates.Add(false);
        }

        if (!panelOpenStates[panelIndex])
        {
            panelOpenStates[panelIndex] = true;

            // 开启 UIManager 的 3D 场景拦截
            UIManager.IsBlocking3DScene = true;

            // 执行滑动动画
            panel.DOKill(true);
            panel.anchoredPosition = panelPositions[panelIndex].hiddenPos;
            panel.DOAnchorPos(panelPositions[panelIndex].visiblePos, slideDuration)
                .SetEase(Ease.OutBack).SetUpdate(true);

            StartCoroutine(ResetClickFlag());
        }
    }

    /// <summary>
    /// 打开指定的面板列表（可以同时打开多个）
    /// </summary>
    public void OpenPanels(List<int> panelIndices)
    {
        if (panelIndices == null || panelIndices.Count == 0) return;

        // 先关闭所有面板，然后再打开指定的
        CloseAllPanels();

        foreach (int idx in panelIndices)
        {
            OpenPanel(idx);
        }
    }

    /// <summary>
    /// 旧方法：打开所有面板（保持兼容性）
    /// </summary>
    public void OpenBillboard()
    {
        // 打开所有面板
        for (int i = 0; i < panels.Count; i++)
        {
            OpenPanel(i);
        }
    }

    /// <summary>
    /// 关闭所有面板
    /// </summary>
    public void CloseAllPanels()
    {
        bool wasAnyOpen = IsAnyPanelOpen();

        for (int i = 0; i < panels.Count; i++)
        {
            if (panelOpenStates.Count > i && panelOpenStates[i])
            {
                panelOpenStates[i] = false;
                RectTransform panel = panels[i];
                
                if (panel != null)
                {
                    panel.DOKill(true);
                    panel.DOAnchorPos(panelPositions[i].hiddenPos, slideDuration)
                        .SetEase(Ease.InBack).SetUpdate(true);
                }
            }
        }

        // 如果之前有面板是开着的，现在关闭，要取消 3D 场景拦截
        if (wasAnyOpen)
        {
            UIManager.IsBlocking3DScene = false;
        }
    }

    /// <summary>
    /// 旧方法：关闭公告栏（保持兼容性）
    /// </summary>
    public void CloseBillboard()
    {
        CloseAllPanels();
    }

    private bool IsAnyPanelOpen()
    {
        foreach (bool state in panelOpenStates)
        {
            if (state) return true;
        }
        return false;
    }

    private bool IsClickingOnAnyOpenPanel()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            if (panelOpenStates.Count > i && panelOpenStates[i] && panels[i] != null)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(panels[i], Input.mousePosition, null))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator ResetClickFlag()
    {
        justOpened = true;
        yield return new WaitForEndOfFrame();
        justOpened = false;
    }
}
