using System;
using UnityEngine;
using UnityEngine.UI;

public class UiPanelHoverEmphasisController : MonoBehaviour
{
    [Serializable]
    private class PanelVisual
    {
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private Image panelImage;
        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;
        [SerializeField, Range(0f, 1f)] private float inactiveAlpha = 0.5f;

        public RectTransform PanelRect => panelRect;
        public Sprite ActiveSprite => activeSprite;
        public Sprite InactiveSprite => inactiveSprite;
        public float InactiveAlpha => inactiveAlpha;

        private float transitionStartAlpha;
        private float transitionTargetAlpha;
        private float currentAlpha;
        private Vector3 baseScale;
        private Vector3 transitionStartScale;
        private Vector3 transitionTargetScale;
        private Vector3 currentScale;
        private float transitionElapsed;
        private bool isTransitioning;

        public void ApplyVisual(float alpha, Sprite sprite)
        {
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = alpha;
            }

            if (panelImage != null)
            {
                panelImage.sprite = sprite;
            }
        }

        public void Initialize()
        {
            currentAlpha = panelCanvasGroup != null ? panelCanvasGroup.alpha : inactiveAlpha;
            baseScale = panelRect != null ? panelRect.localScale : Vector3.one;
            currentScale = baseScale;
            transitionStartAlpha = currentAlpha;
            transitionTargetAlpha = currentAlpha;
            transitionStartScale = currentScale;
            transitionTargetScale = currentScale;
            transitionElapsed = 0f;
            isTransitioning = false;
        }

        public void SetState(bool emphasize, float emphasizedAlpha, float emphasizedScaleMultiplier, bool allowScaleEmphasis, bool instant)
        {
            float targetAlpha = emphasize ? emphasizedAlpha : inactiveAlpha;
            Sprite targetSprite = emphasize ? activeSprite : inactiveSprite;
            Vector3 targetScale = baseScale;

            if (allowScaleEmphasis && emphasize)
            {
                targetScale = baseScale * emphasizedScaleMultiplier;
            }

            if (panelImage != null)
            {
                panelImage.sprite = targetSprite;
            }

            if (panelCanvasGroup == null && panelRect == null)
            {
                return;
            }

            if (instant)
            {
                currentAlpha = targetAlpha;
                transitionStartAlpha = targetAlpha;
                transitionTargetAlpha = targetAlpha;
                currentScale = targetScale;
                transitionStartScale = targetScale;
                transitionTargetScale = targetScale;
                transitionElapsed = 0f;
                isTransitioning = false;

                if (panelCanvasGroup != null)
                {
                    panelCanvasGroup.alpha = targetAlpha;
                }

                if (panelRect != null)
                {
                    panelRect.localScale = targetScale;
                }

                return;
            }

            if (Mathf.Approximately(currentAlpha, targetAlpha) && currentScale == targetScale)
            {
                currentAlpha = targetAlpha;
                transitionStartAlpha = targetAlpha;
                transitionTargetAlpha = targetAlpha;
                currentScale = targetScale;
                transitionStartScale = targetScale;
                transitionTargetScale = targetScale;
                transitionElapsed = 0f;
                isTransitioning = false;

                if (panelCanvasGroup != null)
                {
                    panelCanvasGroup.alpha = targetAlpha;
                }

                if (panelRect != null)
                {
                    panelRect.localScale = targetScale;
                }

                return;
            }

            transitionStartAlpha = currentAlpha;
            transitionTargetAlpha = targetAlpha;
            transitionStartScale = currentScale;
            transitionTargetScale = targetScale;
            transitionElapsed = 0f;
            isTransitioning = true;
        }

        public void TickTransition(float deltaTime, float duration, AnimationCurve curve)
        {
            if (!isTransitioning)
            {
                return;
            }

            if (duration <= 0f)
            {
                currentAlpha = transitionTargetAlpha;
                currentScale = transitionTargetScale;

                if (panelCanvasGroup != null)
                {
                    panelCanvasGroup.alpha = currentAlpha;
                }

                if (panelRect != null)
                {
                    panelRect.localScale = currentScale;
                }

                isTransitioning = false;
                return;
            }

            transitionElapsed += deltaTime;
            float normalizedTime = Mathf.Clamp01(transitionElapsed / duration);
            float easedTime = curve != null ? curve.Evaluate(normalizedTime) : normalizedTime;

            currentAlpha = Mathf.Lerp(transitionStartAlpha, transitionTargetAlpha, easedTime);
            currentScale = Vector3.Lerp(transitionStartScale, transitionTargetScale, easedTime);

            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = currentAlpha;
            }

            if (panelRect != null)
            {
                panelRect.localScale = currentScale;
            }

            if (normalizedTime >= 1f)
            {
                currentAlpha = transitionTargetAlpha;
                currentScale = transitionTargetScale;

                if (panelCanvasGroup != null)
                {
                    panelCanvasGroup.alpha = currentAlpha;
                }

                if (panelRect != null)
                {
                    panelRect.localScale = currentScale;
                }

                isTransitioning = false;
            }
        }
    }

    [Header("Panels")]
    [SerializeField] private PanelVisual[] frontPanels = Array.Empty<PanelVisual>();
    [SerializeField] private PanelVisual backgroundPanel;

    [Header("Visual States")]
    [SerializeField, Range(0f, 1f)] private float emphasizedAlpha = 1f;
    [SerializeField, Min(0f)] private float emphasizedScale = 1.1f;
    [SerializeField, Min(0f)] private float transitionDuration = 0.15f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Optional")]
    [SerializeField] private Canvas parentCanvas;

    private int currentHoveredFrontIndex = -2;

    private void Awake()
    {
        if (parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }
    }

    private void Start()
    {
        InitializePanels();
        UpdatePanelVisuals(forceUpdate: true, instant: true);
    }

    private void Update()
    {
        UpdatePanelVisuals(forceUpdate: false, instant: false);
        TickTransitions();
    }

    private void InitializePanels()
    {
        for (int index = 0; index < frontPanels.Length; index++)
        {
            PanelVisual panel = frontPanels[index];
            if (panel != null)
            {
                panel.Initialize();
            }
        }

        if (backgroundPanel != null)
        {
            backgroundPanel.Initialize();
        }
    }

    private void UpdatePanelVisuals(bool forceUpdate, bool instant)
    {
        int hoveredFrontIndex = GetHoveredFrontPanelIndex();
        if (!forceUpdate && hoveredFrontIndex == currentHoveredFrontIndex)
        {
            return;
        }

        currentHoveredFrontIndex = hoveredFrontIndex;

        for (int index = 0; index < frontPanels.Length; index++)
        {
            bool isHovered = index == hoveredFrontIndex;

            if (isHovered)
            {
                frontPanels[index].PanelRect.SetAsLastSibling();
            }

            ApplyState(frontPanels[index], emphasize: isHovered, allowScaleEmphasis: true, instant: instant);
        }

        bool shouldEmphasizeBackground = hoveredFrontIndex < 0;
        ApplyState(backgroundPanel, emphasize: shouldEmphasizeBackground, allowScaleEmphasis: false, instant: instant);
    }

    private void TickTransitions()
    {
        float deltaTime = Time.unscaledDeltaTime;

        for (int index = 0; index < frontPanels.Length; index++)
        {
            PanelVisual panel = frontPanels[index];
            if (panel != null)
            {
                panel.TickTransition(deltaTime, transitionDuration, transitionCurve);
            }
        }

        if (backgroundPanel != null)
        {
            backgroundPanel.TickTransition(deltaTime, transitionDuration, transitionCurve);
        }
    }

    private void ApplyState(PanelVisual panel, bool emphasize, bool allowScaleEmphasis, bool instant)
    {
        if (panel == null)
        {
            return;
        }

        panel.SetState(emphasize, emphasizedAlpha, emphasizedScale, allowScaleEmphasis, instant);
    }

    private int GetHoveredFrontPanelIndex()
    {
        Camera eventCamera = GetEventCamera();

        for (int index = 0; index < frontPanels.Length; index++)
        {
            PanelVisual panel = frontPanels[index];
            if (panel == null || panel.PanelRect == null)
            {
                continue;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(panel.PanelRect, Input.mousePosition, eventCamera))
            {
                return index;
            }
        }

        return -1;
    }

    private Camera GetEventCamera()
    {
        if (parentCanvas == null)
        {
            return null;
        }

        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }

        return parentCanvas.worldCamera;
    }
}
