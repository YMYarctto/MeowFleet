using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillCard_UI : UIView<SkillCard_UI>
{
    public static int CardID;
    int _id = CardID;
    public override int ID => _id;

    Skill skill;
    public List<Vector2Int> SkillRange => skill.SkillRange;

    private float intensity = 5f;

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

    Vector3 this_initPos;
    RectTransform rect;

    public override void Init()
    {
        card_trans = transform.Find("bg");
        shadow = card_trans.Find("mask").Find("shadow_2");
        rect = GetComponent<RectTransform>();

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
    }

    public void Init(Ship ship)
    {
        skill = Skill.Get(ship,this);
        Sprite sprite = ResourceManager.instance.GetSpriteByType(skill.GetType());
        if(sprite!=null)card_trans.Find("image").GetComponent<Image>().sprite = sprite;
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
        if(pointerLock)
        {
            Wave();
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
        Vector2 mousePos = MouseListener.MousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        PVEController.instance.UICanvas.transform as RectTransform,
        mousePos,
        PVEController.instance.UICanvas.worldCamera,
        out mousePos
        );

        Vector2 dir = mousePos - rect.anchoredPosition;

        float angle = 129f-Vector2.SignedAngle(Vector2.right, dir);
        Debug.Log(angle);

        // 限制旋转角度
        angle = Mathf.Clamp(angle, -intensity, intensity);

        waveTween?.Kill();
        waveTween = card_trans.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, angle), 0.15f).SetEase(Ease.OutQuad);
    }

    private void ReWave()
    {
        waveTween?.Kill();
        waveTween = card_trans.DOLocalRotate(Vector2.zero, 0.15f).SetEase(Ease.OutQuad);
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
        if(is_select)
        {
            return;
        }
        pointerLock=true;

        Wave();
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
        if(SkillArea.GetUIView().IsDragging)
        {
            return;
        }
        is_select = true;
        pointerLock = false;

        clickTween?.Kill();
        clickTween = card_trans.DOScale(new Vector3(1.1f,1.1f,1.1f),0.1f).SetEase(Ease.OutQuad);
        ReWave();

        SkillArea.GetUIView().SelectSkillCard(this);
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
        SkillArea.GetUIView().DisableSkillCard(this);
        PVEController.instance.ResetPVEMap();
        OnSelectEnd();
    }

    private void DoPointerExit()
    {
        pointerLock=false;
        ReWave();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SkillArea.GetUIView().OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SkillArea.GetUIView().OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SkillArea.GetUIView().OnDrag(eventData);
    }

    public static GameObject Create(int id,Transform parent)
    {
        CardID = id;
        var obj = Instantiate(ResourceManager.instance.GetPerfabByType<SkillCard_UI>(),parent,false);
        obj.AddComponent<SkillCard_UI>();
        return obj;
    }
}
