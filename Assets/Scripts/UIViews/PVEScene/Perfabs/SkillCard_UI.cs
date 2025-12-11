using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillCard_UI : UIView
{
    public override UIView currentView => this;

    static int CardID;
    int _id = CardID;
    public override int ID => _id;

    Skill skill;

    private float intensity = 6f;
    private Vector2 duration_range = new(0.1f, 0.25f);

    bool waveLock = false;
    bool pointerLock = false;

    bool is_select = false;
    bool need_exit = false;

    bool isInit = false;

    Transform card_trans;
    Transform shadow;

    Tween waveTween;
    Tween clickTween;
    Tween moveTween;
    EventTrigger eventTrigger;

    Vector3 startPos;
    Vector3 this_initPos;

    public override void Init()
    {
        card_trans = transform.Find("bg");
        shadow = card_trans.Find("mask").Find("shadow_2");
        startPos = shadow.position;

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_pointerEnter = new EventTrigger.Entry();
        entry_pointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_pointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });

        EventTrigger.Entry entry_pointerExit = new EventTrigger.Entry();
        entry_pointerExit.eventID = EventTriggerType.PointerExit;
        entry_pointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });

        EventTrigger.Entry entry_pointerClick = new EventTrigger.Entry();
        entry_pointerClick.eventID = EventTriggerType.PointerClick;
        entry_pointerClick.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });

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

        eventTrigger.triggers.Add(entry_pointerEnter);
        eventTrigger.triggers.Add(entry_pointerExit);
        eventTrigger.triggers.Add(entry_pointerClick);

        Wave();
    }

    public void Init(Ship ship)
    {
        skill = Skill.Get(ship,this);
        startPos = shadow.position;
        this_initPos = transform.localPosition;

        isInit =true;
    }

    public void InitPosition()
    {
        transform.localPosition = this_initPos;
    }

    void FixedUpdate()
    {
        if(!isInit)
        {
            return;
        }
        shadow.rotation = Quaternion.Euler(Vector3.zero);
        shadow.position = new Vector3(startPos.x,shadow.position.y,0);
        if(MouseListener.Distance(transform.position+new Vector3(200,0,0))<400)
        {
            if(!waveLock&&!pointerLock)
            {
                waveLock = true;
                float value = Mathf.Clamp(MouseListener.DistanceY(transform.position), -1, 1);
                Wave(value*MouseListener.MouseSpeed);
            }
        }
        else
        {
            waveLock = false;
        }
    }

    public bool SetActive(float delay=0)
    {
        SetActive(skill.CanSkill,delay);
        return skill.CanSkill;
    }

    public void SetActive(bool active,float delay=0)
    {
        moveTween?.Kill();
        moveTween = transform.DOLocalMoveX(active?-215:-915,0.3f).SetDelay(active?delay:0).SetEase(Ease.InOutQuad);
    }

    private void Wave()
    {
        float targetAngle = Random.Range(-intensity, intensity);
        float duration = Random.Range(duration_range.x, duration_range.y);

        waveTween?.Kill();

        waveTween = card_trans.DOLocalRotate(new Vector3(0, 0, targetAngle), duration).SetEase(Ease.OutQuad);
    }
    
    private void Wave(float range)
    {
        float new_internsity = Mathf.Clamp(range,-intensity,intensity);
        Vector2 v2 = new_internsity > 0 ? new Vector2(0, new_internsity) : new Vector2(new_internsity, 0);
        float targetAngle = Random.Range(v2.x, v2.y);
        float duration = Random.Range(duration_range.x, duration_range.y);

        waveTween?.Kill();

        waveTween = card_trans.DOLocalRotate(new Vector3(0, 0, targetAngle), duration).SetEase(Ease.OutQuad);
    }

    public void MoveUp_Animation(float value,float delay=0)
    {
        moveTween?.Kill();
        moveTween = transform.DOLocalMoveY(transform.localPosition.y+value,0.3f).SetDelay(delay).SetEase(Ease.InOutQuad);
    }

    public void MoveUp(float value)
    {
        transform.localPosition += new Vector3(0,value,0);
    }

    private void OnPointerEnter(PointerEventData eventData)
    {
        pointerLock=true;

        waveTween?.Kill();
        waveTween = card_trans.DOLocalRotate(Vector3.zero, 0.15f).SetEase(Ease.OutQuad);
    }

    private void OnPointerExit(PointerEventData eventData)
    {
        if (is_select)
        {
            need_exit = true;
            return;
        }

        DoPointerExit();
    }

    private void OnPointerClick(PointerEventData eventData)
    {
        if(UIManager.instance.GetUIView<SkillArea>().IsDragging)
        {
            return;
        }
        is_select = true;

        clickTween?.Kill();
        clickTween = card_trans.DOScale(new Vector3(1.1f,1.1f,1.1f),0.1f).SetEase(Ease.OutQuad);

        UIManager.instance.GetUIView<SkillArea>().SelectSkillCard(this);
        skill.OnSelect();
    }

    public void OnSelectEnd()
    {
        is_select = false;
        if(need_exit)
        {
            DoPointerExit();
            need_exit = false;
        }

        clickTween?.Kill();
        clickTween = card_trans.DOScale(Vector3.one,0.1f).SetEase(Ease.OutQuad);
    }

    public void OnSkillEnd()
    {
        UIManager.instance.GetUIView<SkillArea>().DisableSkillCard(this);
        PVEController.instance.ResetPVEMap();
        OnSelectEnd();
    }

    private void DoPointerExit()
    {
        pointerLock=false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<SkillArea>().OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<SkillArea>().OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<SkillArea>().OnDrag(eventData);
    }

    public static GameObject Create(int id,Transform parent)
    {
        CardID = id;
        var obj = Instantiate(ResourceManager.instance.GetPerfabByType<SkillCard_UI>(),parent,false);
        obj.AddComponent<SkillCard_UI>();
        return obj;
    }
}
