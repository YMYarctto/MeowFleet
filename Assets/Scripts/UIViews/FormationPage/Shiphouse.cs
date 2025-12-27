using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shiphouse : UIView
{
    List<ShipContainer> ship_list;

    RectTransform viewport;
    RectTransform content;

    float rubberPower = 0.2f;
    float inertia = 0.90f;
    float reboundDuration = 0.35f;
    Ease reboundEase = Ease.OutQuad;

    float seq_duration=0.3f;

    float lastMouseX;
    float velocityX;
    bool isDragging;

    Tweener reboundTween;
    Sequence decreaseSeq;
    EventTrigger eventTrigger;

    Vector2 content_init = new(0,307);
    Vector3 ship_pos = new(50,0,0);
    const float CONTAINER_GAP = 50f;

    public override UIView currentView => this;

    public override void Init()
    {
        Transform shiphouse = GameObject.Find("Shiphouse").transform;
        viewport = shiphouse.Find("viewport").GetComponent<RectTransform>();
        content = shiphouse.Find("content").GetComponent<RectTransform>();

        ship_list??=new();
        content = transform.Find("content").GetComponent<RectTransform>();
        viewport = GetComponent<RectTransform>();

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
        ShipManager.instance.New();
        foreach(var kv in ShipManager.instance.Shiphouse)
        {
            AddShipUI(kv.Key,kv.Value);
        }
    }

    void Update()
    {
        if (!isDragging)
        {
            // 惯性移动
            if (Mathf.Abs(velocityX) > 5f)
            {
                MoveContent(velocityX * Time.deltaTime);
                velocityX *= inertia;
            }
        }
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

        // 内容短时加入阻力
        if (content.rect.width<viewport.rect.width || content.anchoredPosition.x <= viewport.anchoredPosition.x || content.anchoredPosition.x + content.rect.width >= viewport.anchoredPosition.x + viewport.rect.width)
            delta *= rubberPower;

        MoveContent(delta);
        velocityX = delta / Time.deltaTime; // 记录速度
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

    void TryRebound()
    {
        float contentWidth = content.rect.width;
        float viewWidth = viewport.rect.width;

        // 内容比可视区域短 → 一定回弹到 x = 0
        if (contentWidth <= viewWidth)
        {
            PlayRebound(Vector2.zero);
            return;
        }

        if (content.anchoredPosition.x < 0)
        {
            PlayRebound(new Vector2(content.anchoredPosition.x, 0));
            return;
        }

        float max = viewWidth-contentWidth;
        if (content.anchoredPosition.x > max)
        {
            PlayRebound(new Vector2(content.anchoredPosition.x, max));
            return;
        }
    }

    // DOTween 回弹
    void PlayRebound(Vector2 target)
    {
        reboundTween?.Kill();

        reboundTween = content.DOAnchorPos(target, reboundDuration).SetEase(reboundEase);
    }
}
