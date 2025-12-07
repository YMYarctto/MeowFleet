using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillArea : UIView
{
    RectTransform content;
    RectTransform viewport;

    float rubberPower = 0.4f;
    float inertia = 0.90f;
    float reboundDuration = 0.35f;
    Ease reboundEase = Ease.OutQuad;

    float lastMouseY;
    float velocityY;
    bool isDragging;
    public bool IsDragging=>isDragging;

    Tweener reboundTween;
    EventTrigger eventTrigger;

    Vector2 content_init = new(600,450);
    Vector3 card_pos = new(-215,-340,0);
    const float CARD_GAP = 271f;

    List<SkillCard_UI> skill_list;
    SkillCard_UI current_select;

    public override UIView currentView => this;

    public override void Init()
    {
        skill_list??=new();
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
            }
        }
    }

    public void AddSkillCard(int id,Ship ship)
    {
        skill_list??=new();
        GameObject card = SkillCard_UI.Create(id,content);
        SkillCard_UI card_ui = card.GetComponent<SkillCard_UI>();
        card.transform.localPosition=card_pos;
        card_pos.y-=CARD_GAP;
        card_ui.Init(ship);
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
    }

    public void ShowSkill()
    {
        content.sizeDelta = content_init;
        skill_list.ForEach(ui =>ui.InitPosition());
        float delay = 0f;
        for(int i=0;i<skill_list.Count;i++)
        {
            if(!skill_list[i].SetActive(delay))
            {
                for(int j=i+1;j<skill_list.Count;j++)
                {
                    skill_list[j].MoveUp(CARD_GAP);
                }
            }
            else
            {
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

        // 停止回弹动画，防止拖动时卡顿
        reboundTween?.Kill();

        velocityY = 0;
        lastMouseY = eventData.position.y;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float delta = eventData.position.y - lastMouseY;
        lastMouseY = eventData.position.y;

        // 内容短时加入阻力
        if (content.anchoredPosition.y <= viewport.anchoredPosition.y|| content.anchoredPosition.y - content.rect.height >= viewport.anchoredPosition.y - viewport.rect.height)
            delta *= rubberPower;

        MoveContent(delta);
        velocityY = delta / Time.deltaTime; // 记录速度
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

        reboundTween = content.DOAnchorPos(target, reboundDuration).SetEase(reboundEase);
    }
}
