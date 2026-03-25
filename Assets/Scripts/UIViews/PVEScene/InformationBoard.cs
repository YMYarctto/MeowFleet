using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InformationBoard : UIView<InformationBoard>
{
    const float CARD_GAP = 160f;
    const float MIN_INERTIA_VELOCITY = 5f;

    readonly Vector3 card_start_pos = new(300,0,0);
    readonly Vector2 init_size = new(590,0);

    readonly List<InformationCard_UI> card_list = new();

    float rubberPower = 0.2f;
    float inertia = 0.90f;
    float reboundDuration = 0.35f;
    float scrollPower = 1f;
    Ease reboundEase = Ease.OutQuad;

    float lastMouseY;
    float velocityY;
    RectTransform content;
    RectTransform viewport;
    InfomationRollBar infomationRollBar;

    Tweener reboundTween;
    EventTrigger eventTrigger;
    Camera eventCamera;
    Vector3 nextCardPos;

    bool isDragging;
    bool scrollRegistered;
    public bool IsDragging => isDragging;

    int round;
    string stage;

    protected override UnityAction WaitForAllUIViewAdded => SetUpRollBar;

    float MaxScrollOffsetY => Mathf.Max(0f, content.rect.height - viewport.rect.height);
    bool CanScroll => content.rect.height > viewport.rect.height;

    public float UnviewLength => MaxScrollOffsetY;
    public float CurrentLength => Mathf.Clamp(content.anchoredPosition.y,0,UnviewLength);
    public float ViewLength => viewport.rect.height;
    public float TotalLength => ViewLength + UnviewLength;

    public override void Init()
    {
        InformationCard_UI.ResetID();
        viewport = transform.Find("viewport").GetComponent<RectTransform>();
        content = viewport.Find("content").GetComponent<RectTransform>();
        eventCamera = GetEventCamera();
        nextCardPos = card_start_pos;

        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = init_size;
        ConfigureDragTrigger();
    }

    void Update()
    {
        if (isDragging || Mathf.Abs(velocityY) <= MIN_INERTIA_VELOCITY)
        {
            if (!isDragging && velocityY != 0f)
            {
                velocityY = 0f;
                if (IsOutOfBounds())
                {
                    TryRebound();
                }
            }
            return;
        }

        MoveContent(velocityY * Time.deltaTime);
        velocityY *= IsOutOfBounds() ? inertia * rubberPower : inertia;
        RefreshRollBar();
    }

    public void SetRound(int r)
    {
        round = r;
    }

    public void SetStage(PVEController.PVEState s)
    {
        stage = s switch
        {
            PVEController.PVEState.PlayerAttack => "齐射阶段",
            PVEController.PVEState.PlayerSkill => "技能阶段",
            PVEController.PVEState.EnemyAttack => "敌方回合",
            _ => "敌方回合",
        };
    }

    public void Set(float per)
    {
        reboundTween?.Kill();
        velocityY = 0f;
        float targetY = Mathf.Clamp01(per) * UnviewLength;
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetY);
        RefreshRollBar();
    }

    public string GetTitle()
    {
        return $"第 {round} 回合：{stage}";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        reboundTween?.Kill();

        velocityY = 0;
        lastMouseY = eventData.position.y;

        RefreshRollBar();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float delta = eventData.position.y - lastMouseY;
        lastMouseY = eventData.position.y;

        if (ShouldApplyRubber())
        {
            delta *= rubberPower;
        }

        MoveContent(delta);
        velocityY = Time.deltaTime > 0f ? delta / Time.deltaTime : 0f;
        RefreshRollBar();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        TryRebound();
    }

    void MoveContent(float delta)
    {
        Vector2 pos = content.anchoredPosition;
        pos.y += delta;
        content.anchoredPosition = pos;
    }

    void TryRebound()
    {
        if (!CanScroll)
        {
            PlayRebound(Vector2.zero);
            return;
        }

        if (content.anchoredPosition.y < 0)
        {
            PlayRebound(new Vector2(content.anchoredPosition.x, 0));
            return;
        }

        float max = MaxScrollOffsetY;
        if (content.anchoredPosition.y > max)
        {
            PlayRebound(new Vector2(content.anchoredPosition.x, max));
        }
    }

    void PlayRebound(Vector2 target)
    {
        reboundTween?.Kill();
        if (Vector2.Distance(content.anchoredPosition, target) <= Mathf.Epsilon)
        {
            content.anchoredPosition = target;
            RefreshRollBar();
            return;
        }

        reboundTween = content.DOAnchorPos(target, reboundDuration).SetEase(reboundEase).OnUpdate(RefreshRollBar);
    }

    public void MoveToEnd()
    {
        if (!CanScroll)
        {
            content.anchoredPosition = Vector2.zero;
            RefreshRollBar();
            return;
        }

        PlayRebound(new Vector2(content.anchoredPosition.x, MaxScrollOffsetY));
    }

    public InformationCard_UI NewInformation()
    {
        InformationCard_UI.PrepareNextID();
        GameObject card = Instantiate(ResourceManager.instance.GetPerfabByType<InformationCard_UI>(), content, false);
        InformationCard_UI card_ui = card.GetComponent<InformationCard_UI>();
        card.transform.localPosition = nextCardPos;
        nextCardPos.y -= CARD_GAP;
        card_list.Add(card_ui);
        content.sizeDelta = new Vector2(content.sizeDelta.x,content.sizeDelta.y+CARD_GAP);
        MoveToEnd();

        return card_ui;
    }

    void ConfigureDragTrigger()
    {
        eventTrigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
        eventTrigger.triggers ??= new List<EventTrigger.Entry>();
        eventTrigger.triggers.Clear();

        AddTrigger(EventTriggerType.BeginDrag, data => OnBeginDrag((PointerEventData)data));
        AddTrigger(EventTriggerType.EndDrag, data => OnEndDrag((PointerEventData)data));
        AddTrigger(EventTriggerType.Drag, data => OnDrag((PointerEventData)data));
    }

    void AddTrigger(EventTriggerType eventType, UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = eventType
        };
        entry.callback.AddListener(callback);
        eventTrigger.triggers.Add(entry);
    }

    void RefreshRollBar()
    {
        infomationRollBar?.Set();
    }

    void SetUpRollBar()
    {
        infomationRollBar = InfomationRollBar.GetUIView();
        RefreshRollBar();
    }

    bool ShouldApplyRubber()
    {
        return !CanScroll || IsOutOfBounds();
    }

    bool IsOutOfBounds()
    {
        float offsetY = content.anchoredPosition.y;
        return offsetY < 0f || offsetY > MaxScrollOffsetY;
    }

    void OnDisable()
    {
        UnregisterScrollInput();
        reboundTween?.Kill();
        isDragging = false;
        velocityY = 0f;
    }

    void OnEnable()
    {
        RegisterScrollInput();
    }

    void RegisterScrollInput()
    {
        if (scrollRegistered || InputController.InputAction == null)
        {
            return;
        }

        InputController.InputAction.System.Scroll.performed += OnScroll;
        scrollRegistered = true;
    }

    void UnregisterScrollInput()
    {
        if (!scrollRegistered || InputController.InputAction == null)
        {
            return;
        }

        InputController.InputAction.System.Scroll.performed -= OnScroll;
        scrollRegistered = false;
    }

    void OnScroll(InputAction.CallbackContext context)
    {
        if (isDragging || !IsPointerOverViewport())
        {
            return;
        }

        float scrollY = context.ReadValue<Vector2>().y;
        if (Mathf.Abs(scrollY) <= Mathf.Epsilon)
        {
            return;
        }

        reboundTween?.Kill();
        velocityY = 0f;
        MoveContent(-scrollY * scrollPower);
        RefreshRollBar();
        TryRebound();
    }

    bool IsPointerOverViewport()
    {
        Vector2 pointerPosition = Mouse.current?.position.ReadValue()
            ?? new Vector2(MouseListener.MousePosition.x, MouseListener.MousePosition.y);
        return RectTransformUtility.RectangleContainsScreenPoint(viewport, pointerPosition, eventCamera);
    }

    Camera GetEventCamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }

        return canvas.worldCamera;
    }
}
