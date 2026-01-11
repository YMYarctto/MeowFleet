using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseButton_Setting : BaseButton_Default
{
    GameObject focus;

    private bool isPressing = false;
    private bool needExit = false;
    protected bool isSelect = false;

    public override void Init()
    {
        base.Init();

        Transform focus_trans = transform.Find("focus");
        if(focus_trans)
        {
            focus = focus_trans.gameObject;
            if(focus_trans.TryGetComponent<Image>(out var image)) ResetImage(image);
            focus.SetActive(false);
        }

        EventTrigger.Entry entry_pointerEnter = new EventTrigger.Entry();
        entry_pointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_pointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });

        EventTrigger.Entry entry_pointerExit = new EventTrigger.Entry();
        entry_pointerExit.eventID = EventTriggerType.PointerExit;
        entry_pointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });

        eventTrigger.triggers.Add(entry_pointerEnter);
        eventTrigger.triggers.Add(entry_pointerExit);
    }

    protected override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        isPressing = true;
        needExit = false;
    }

    protected override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isPressing = false;

        if (needExit)
        {
            DoExit();
        }
    }

    private void OnPointerEnter(PointerEventData eventData)
    {
        if(isSelect)return;
        DoEnter();
        needExit = false;
    }

    private void OnPointerExit(PointerEventData eventData)
    {
        if (isPressing)
        {
            needExit = true;
            return;
        }

        DoExit();
    }

    protected virtual void DoEnter()
    {
        if (!focus) return;
        focus.SetActive(true);
    }

    protected virtual void DoExit()
    {
        if (!focus) return;
        focus.SetActive(false);
    }
}
