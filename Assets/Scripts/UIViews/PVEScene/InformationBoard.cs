using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class InformationBoard : UIView
{
    public override UIView currentView => this;

    public static int CardID=0;

    float rubberPower = 0.6f;
    float inertia = 0.90f;
    float reboundDuration = 0.35f;
    Ease reboundEase = Ease.OutQuad;

    float lastMouseY;
    float velocityY;
    bool isDragging;
    public bool IsDragging=>isDragging;

    Vector3 card_pos = new(300,0,0);
    Vector2 init_size = new(590,0);
    const float CARD_GAP = 160f;

    RectTransform content;
    RectTransform viewport;
    InfomationRollBar infomationRollBar;

    Tweener reboundTween;
    EventTrigger eventTrigger;

    int round;
    string stage;

    public float UnviewLength => content.rect.height<=viewport.rect.height?0:content.rect.height - viewport.rect.height;
    public float CurrentLength => Mathf.Clamp(content.anchoredPosition.y,0,UnviewLength);
    public float ViewLength => viewport.rect.height;
    public float TotalLength => ViewLength + UnviewLength;

    List<InformationCard_UI> card_list;

    public override void Init()
    {
        card_list??=new();
        viewport = transform.Find("viewport").GetComponent<RectTransform>();
        content = viewport.Find("content").GetComponent<RectTransform>();

        content.sizeDelta = init_size;

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
    }

    void Start()
    {
        infomationRollBar = UIManager.instance.GetUIView<InfomationRollBar>();
        infomationRollBar.Set();
    }

    void Update()
    {
        if (!isDragging)
        {
            // 惯性移动
            if (Mathf.Abs(velocityY) > 5f)
            {
                MoveContent(velocityY * Time.deltaTime);
                velocityY *= inertia;
                if (content.anchoredPosition.y <= viewport.anchoredPosition.y|| content.anchoredPosition.y - content.rect.height >= viewport.anchoredPosition.y - viewport.rect.height)
                    velocityY *= rubberPower;
                infomationRollBar.Set();
            }
        }
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
        float targetY = per * UnviewLength;
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetY);
    }

    public string GetTitle()
    {
        return $"第 {round} 回合：{stage}";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        // 停止回弹动画，防止拖动时卡顿
        reboundTween?.Kill();

        velocityY = 0;
        lastMouseY = eventData.position.y;

        infomationRollBar.Set();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float delta = eventData.position.y - lastMouseY;
        lastMouseY = eventData.position.y;

        // 内容短时加入阻力
        if (content.anchoredPosition.y <= viewport.anchoredPosition.y|| content.anchoredPosition.y - content.rect.height >= viewport.anchoredPosition.y - viewport.rect.height)
            delta *= rubberPower;

        MoveContent(delta);
        velocityY = Mathf.Clamp(delta / Time.deltaTime,-30,30); // 记录速度
        infomationRollBar.Set();
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
        float contentHeight = content.rect.height;
        float viewHeight = viewport.rect.height;

        // 内容比可视区域短 → 一定回弹到 x = 0
        if (contentHeight <= viewHeight)
        {
            PlayRebound(Vector2.zero);
            return;
        }

        if (content.anchoredPosition.y < 0)
        {
            PlayRebound(new Vector2(content.anchoredPosition.x, 0));
            return;
        }

        float max = contentHeight - viewHeight;
        if (content.anchoredPosition.y > max)
        {
            PlayRebound(new Vector2(content.anchoredPosition.x, max));
            return;
        }
    }

    // DOTween 回弹
    void PlayRebound(Vector2 target)
    {
        reboundTween?.Kill();

        reboundTween = content.DOAnchorPos(target, reboundDuration).SetEase(reboundEase).OnUpdate(()=>infomationRollBar.Set());
    }

    public void MoveToEnd()
    {
        float contentHeight = content.rect.height;
        float viewHeight = viewport.rect.height;

        if (contentHeight <= viewHeight)
        {
            return;
        }

        float max = contentHeight - viewHeight;
        PlayRebound(new Vector2(content.anchoredPosition.x, max));
    }

    public InformationCard_UI NewInformation()
    {
        GameObject card = Instantiate(ResourceManager.instance.GetPerfabByType<InformationCard_UI>(), content);
        InformationCard_UI card_ui = card.GetComponent<InformationCard_UI>();
        CardID++;
        card.transform.localPosition=card_pos;
        card_pos.y-=CARD_GAP;
        card_list.Add(card_ui);
        content.sizeDelta = new Vector2(content.sizeDelta.x,content.sizeDelta.y+CARD_GAP);
        MoveToEnd();

        return card_ui;
    }
}
