using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AreaPart : UIView
{
    public override UIView currentView => this;
    public override int ID => id;

    int id = AreaSelectController.AreaID;
    GameObject pointer;
    Pointer_Animation pointer_Animation;
    EventTrigger eventTrigger;

    public override void Init()
    {
        var img = GetComponent<Image>();
        img.alphaHitTestMinimumThreshold = 0.1f;

        pointer = Instantiate(AreaSelectController.instance.PointerPrefab);
        pointer_Animation = pointer.GetComponent<Pointer_Animation>();
        Transform trans = pointer.transform;
        trans.SetParent(transform.Find("pointer_trans"), true);
        trans.localPosition = Vector3.zero;
        trans.SetParent(AreaSelectController.instance.UI_trans, true);

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_pointerEnter = new EventTrigger.Entry();
        entry_pointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_pointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });

        EventTrigger.Entry entry_pointerEixt = new EventTrigger.Entry();
        entry_pointerEixt.eventID = EventTriggerType.PointerExit;
        entry_pointerEixt.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });

        eventTrigger.triggers.Add(entry_pointerEnter);
        eventTrigger.triggers.Add(entry_pointerEixt);
    }

    public override void Enable()
    {
        base.Enable();
        pointer.SetActive(true);
    }

    public override void Disable()
    {
        pointer.SetActive(false);
        base.Disable();
    }

    private void OnPointerEnter(PointerEventData eventData)
    {
        pointer_Animation.PlayForward();
    }

    private void OnPointerExit(PointerEventData eventData)
    {
        pointer_Animation.PlayBackward();
    }
}
