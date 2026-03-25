using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SkillArea : UIView<SkillArea>
{
    const float CARD_GAP = 271f;
    const float MIN_INERTIA_VELOCITY = 5f;

    readonly Vector2 content_init = new(600,450);
    readonly Vector3 card_start_pos = new(-915,-340,0);

    readonly List<SkillCard_UI> skill_list = new();

    RectTransform content;
    RectTransform viewport;
    EventTrigger eventTrigger;

    float rubberPower = 0.2f;
    float inertia = 0.90f;
    float reboundDuration = 0.35f;
    float scrollPower = 1f;
    float lastMouseY;
    float velocityY;
    Tweener reboundTween;
    Ease reboundEase = Ease.OutQuad;

    Camera eventCamera;
    Vector3 nextCardPos;
    bool isDragging;
    bool scrollRegistered;
    public bool IsDragging => isDragging;

    SkillCard_UI current_select;

    float MaxScrollOffsetY => Mathf.Max(0f, content.rect.height - viewport.rect.height);
    bool CanScroll => content.rect.height > viewport.rect.height;

    public override void Init()
    {
        content = transform.Find("content").GetComponent<RectTransform>();
        viewport = GetComponent<RectTransform>();
        eventCamera = GetEventCamera();
        nextCardPos = card_start_pos;

        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = content_init;
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
    }

    public void AddSkillCard(int id,Ship ship)
    {
        GameObject card = SkillCard_UI.Create(id,content);
        SkillCard_UI card_ui = card.GetComponent<SkillCard_UI>();
        card.transform.localPosition = nextCardPos;
        nextCardPos.y -= CARD_GAP;
        card_ui.Init(ship);
        card.GetComponentInChildren<SkillRange_UI>().Init(card_ui.SkillRange);
        skill_list.Add(card_ui);
        content.sizeDelta = new Vector2(content.sizeDelta.x,content.sizeDelta.y+CARD_GAP);
    }

    public void SelectSkillCard(SkillCard_UI card)
    {
        if(current_select==card)
        {
            return;
        }
        current_select?.OnSelectEnd();
        current_select=card;
    }

    public void DisableSkillCard(SkillCard_UI card)
    {
        card.SetActive(false);
        if(current_select==card)
        {
            current_select=null;
        }
        int index = skill_list.IndexOf(card);
        if(index<0)
        {
            Debug.LogError("找不到SkillCard");
            return;
        }
        for(int i=index+1;i<skill_list.Count;i++)
        {
            skill_list[i].MoveUp_Animation(CARD_GAP,0.2f);
        }
        content.sizeDelta = new Vector2(content.sizeDelta.x, Mathf.Max(content_init.y, content.sizeDelta.y - CARD_GAP));
        if (!isDragging)
        {
            TryRebound();
        }
    }

    public void ClearSelectedCard()
    {
        current_select?.OnSelectEnd();
        current_select = null;
    }

    public void ShowSkill()
    {
        reboundTween?.Kill();
        velocityY = 0f;
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = content_init;

        int disabledCount = 0;
        float delay = 0f;
        for(int i=0;i<skill_list.Count;i++)
        {
            SkillCard_UI skillCard = skill_list[i];
            skillCard.InitPosition();

            if(!skillCard.SetActive(delay))
            {
                disabledCount++;
            }
            else
            {
                if (disabledCount > 0)
                {
                    skillCard.MoveUp(CARD_GAP * disabledCount);
                }
                delay+=0.1f;
                content.sizeDelta = new Vector2(content.sizeDelta.x,content.sizeDelta.y+CARD_GAP);
            }
        }
    }

    public void ShowSkill(bool active)
    {
        skill_list.ForEach(ui =>ui.SetActive(active));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        reboundTween?.Kill();

        velocityY = 0;
        lastMouseY = eventData.position.y;
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
            return;
        }

        reboundTween = content.DOAnchorPos(target, reboundDuration).SetEase(reboundEase);
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
