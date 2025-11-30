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
    Sequence pointer_seq;
    EventTrigger eventTrigger;

    Vector3 startPos;

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

        eventTrigger.triggers.Add(entry_pointerEnter);
        eventTrigger.triggers.Add(entry_pointerExit);
        eventTrigger.triggers.Add(entry_pointerClick);

        Wave();
    }

    public void Init(Ship ship)
    {
        skill = Skill.Get(ship,this);
        startPos = shadow.position;

        isInit =true;
    }

    void FixedUpdate()
    {
        if(!isInit)
        {
            return;
        }
        shadow.rotation = Quaternion.Euler(Vector3.zero);
        shadow.position = startPos;
        if(MouseListener.MouseMove&&MouseListener.Distance(transform.position+new Vector3(200,0,0))<400)
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

    public void SetActive()
    {
        SetActive(skill.CanSkill);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
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

    private void OnPointerEnter(PointerEventData eventData)
    {
        pointerLock=true;
        pointer_seq?.Kill();
        pointer_seq = DOTween.Sequence();
        pointer_seq.Append(card_trans.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.OutQuad));
        pointer_seq.Join(card_trans.DOScale(new Vector3(1.1f,1.1f,1.1f),0.1f).SetEase(Ease.OutQuad));
        pointer_seq.Play();
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
        // if(is_select)
        // {
        //     is_select = false;
        //     PVEController.instance.ClearSelectedSkill();
        //     return;
        // }
        is_select = true;
        skill.OnSelect();
    }

    public void OnSelectEnd()
    {
        is_select = false;
        if(need_exit)
        {
            DoPointerExit();
            SetActive(false);
        }
    }

    private void DoPointerExit()
    {
        pointerLock=false;
        pointer_seq?.Kill();
        pointer_seq = DOTween.Sequence();
        pointer_seq.Join(card_trans.DOScale(Vector3.one,0.1f).SetEase(Ease.OutQuad));
        pointer_seq.Play();
    }

    public static GameObject Create(int id,Ship ship,Transform parent)
    {
        CardID = id;
        var obj = Instantiate(ResourceManager.instance.GetPerfabByType<SkillCard_UI>(),parent,false);
        var ui = obj.AddComponent<SkillCard_UI>();
        ui.Init(ship);
        return obj;
    }
}
