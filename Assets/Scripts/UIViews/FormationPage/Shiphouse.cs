using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Shiphouse : UIView<Shiphouse>
{
    const float MIN_INERTIA_VELOCITY = 5f;

    List<ShipContainer> ship_list;

    RectTransform viewport;
    RectTransform content;

    float rubberPower = 0.2f;
    float inertia = 0.90f;
    float reboundDuration = 0.35f;
    float scrollPower = 1f;
    Ease reboundEase = Ease.OutQuad;

    float seq_duration=0.3f;

    float lastMouseX;
    float velocityX;
    bool isDragging;

    Tweener reboundTween;
    Sequence decreaseSeq;
    EventTrigger eventTrigger;
    Camera eventCamera;
    bool scrollRegistered;

    Vector2 content_init = new(0,307);
    Vector3 ship_pos = new(50,0,0);
    const float CONTAINER_GAP = 50f;

    float MaxScrollOffsetX => Mathf.Max(0f, content.rect.width - viewport.rect.width);
    bool CanScroll => content.rect.width > viewport.rect.width;

    public override void Init()
    {
        Transform shiphouse = GameObject.Find("Shiphouse").transform;
        viewport = shiphouse.Find("viewport").GetComponent<RectTransform>();
        content = shiphouse.Find("content").GetComponent<RectTransform>();

        ship_list??=new();
        content = transform.Find("content").GetComponent<RectTransform>();
        viewport = GetComponent<RectTransform>();
        eventCamera = GetEventCamera();

        content.sizeDelta = content_init;

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_beginDrag = new EventTrigger.Entry();
        entry_beginDrag.eventID = EventTriggerType.BeginDrag;
        entry_beginDrag.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });

        EventTrigger.Entry entry_endDrag = new EventTrigger.Entry();
        entry_endDrag.eventID = EventTriggerType.EndDrag;
        entry_endDrag.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });

        EventTrigger.Entry entry_onDrag = new EventTrigger.Entry();
        entry_onDrag.eventID = EventTriggerType.Drag;
        entry_onDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });

        eventTrigger.triggers.Add(entry_beginDrag);
        eventTrigger.triggers.Add(entry_endDrag);
        eventTrigger.triggers.Add(entry_onDrag);
        // Disable();
    }

    void OnEnable()
    {
        //Temp
        ShipManager.instance.New();
        foreach(var kv in ShipManager.instance.Shiphouse)
        {
            AddShipUI(kv.Key,kv.Value);
        }

        RegisterScrollInput();
    }

    void OnDisable()
    {
        UnregisterScrollInput();
        reboundTween?.Kill();
        isDragging = false;
        velocityX = 0f;
    }

    void Update()
    {
        if (isDragging || Mathf.Abs(velocityX) <= MIN_INERTIA_VELOCITY)
        {
            if (!isDragging && velocityX != 0f)
            {
                velocityX = 0f;
                if (IsOutOfBounds())
                {
                    TryRebound();
                }
            }
            return;
        }

        MoveContent(velocityX * Time.deltaTime);
        velocityX *= IsOutOfBounds() ? inertia * rubberPower : inertia;
    }

    public void AddShipUI(int id,Ship ship)
    {
        ship_list??=new();
        ShipContainer ship_container = ShipContainer.Create(id,ship, content);
        Vector2 sizeDelta= ship_container.rectTransform.sizeDelta;
        ship_container.rectTransform.anchoredPosition=ship_pos+new Vector3(sizeDelta.x/2,0,0);
        
        ship_pos.x+=CONTAINER_GAP+sizeDelta.x;
        ship_list.Add(ship_container);
        content.sizeDelta = new Vector2(content.sizeDelta.x+CONTAINER_GAP+sizeDelta.x,content.sizeDelta.y);
    }

    public void AddShipUI(Ship_Formation ship_ui)
    {
        ship_list??=new();
        ShipContainer ship_container = ShipContainer.Create(ship_ui, content);
        Vector2 sizeDelta= ship_container.rectTransform.sizeDelta;
        ship_container.rectTransform.anchoredPosition=ship_pos+new Vector3(sizeDelta.x/2,0,0);
        
        ship_pos.x+=CONTAINER_GAP+sizeDelta.x;
        ship_list.Add(ship_container);
        content.sizeDelta = new Vector2(content.sizeDelta.x+CONTAINER_GAP+sizeDelta.x,content.sizeDelta.y);
    }

    public void DecreaseShipUi(ShipContainer container)
    {
        int index = ship_list.IndexOf(container);
        if(index<0)
        {
            return;
        }
        decreaseSeq?.Complete();
        decreaseSeq = DOTween.Sequence();
        float delta = container.rectTransform.sizeDelta.x+CONTAINER_GAP;
        for(int i=index+1;i<ship_list.Count;i++)
        {
            float target = ship_list[i].transform.position.x - delta;
            decreaseSeq.Insert(0,ship_list[i].transform.DOMoveX(target,seq_duration).SetEase(Ease.InOutQuad));
        }
        ship_list.Remove(container);
        ship_pos-=new Vector3(delta,0,0);
        decreaseSeq.Play();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        // 停止回弹动画，防止拖动时卡顿
        reboundTween?.Kill();

        velocityX = 0;
        lastMouseX = eventData.position.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float delta = eventData.position.x - lastMouseX;
        lastMouseX = eventData.position.x;

        if (ShouldApplyRubber())
            delta *= rubberPower;

        MoveContent(delta);
        velocityX = Time.deltaTime > 0f ? delta / Time.deltaTime : 0f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        TryRebound();
    }

    void MoveContent(float delta)
    {
        Vector2 pos = content.anchoredPosition;
        pos.x += delta;
        content.anchoredPosition = pos;
    }

    bool ShouldApplyRubber()
    {
        return !CanScroll || IsOutOfBounds();
    }

    bool IsOutOfBounds()
    {
        float offsetX = content.anchoredPosition.x;
        return offsetX > 0f || offsetX < -MaxScrollOffsetX;
    }

    void TryRebound()
    {
        if (!CanScroll)
        {
            PlayRebound(new Vector2(0f, content.anchoredPosition.y));
            return;
        }

        if (content.anchoredPosition.x > 0f)
        {
            PlayRebound(new Vector2(0f, content.anchoredPosition.y));
            return;
        }

        float minX = -MaxScrollOffsetX;
        if (content.anchoredPosition.x < minX)
        {
            PlayRebound(new Vector2(minX, content.anchoredPosition.y));
            return;
        }
    }

    void PlayRebound(Vector2 target)
    {
        reboundTween?.Kill();

        reboundTween = content.DOAnchorPos(target, reboundDuration).SetEase(reboundEase);
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
        velocityX = 0f;
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
