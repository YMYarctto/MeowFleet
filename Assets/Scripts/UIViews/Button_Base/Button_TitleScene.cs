using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Button_TitleScene : UIView
{
    EventTrigger eventTrigger;
    Image image;
    Color color;

    public override void Init()
    {
        image = GetComponent<Image>();
        color = image.color;

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_pointerClick = new EventTrigger.Entry();
        entry_pointerClick.eventID = EventTriggerType.PointerClick;
        entry_pointerClick.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });

        EventTrigger.Entry entry_pointerEnter = new EventTrigger.Entry();
        entry_pointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_pointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });

        EventTrigger.Entry entry_pointerExit = new EventTrigger.Entry();
        entry_pointerExit.eventID = EventTriggerType.PointerExit;
        entry_pointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });

        eventTrigger.triggers.Add(entry_pointerClick);
        eventTrigger.triggers.Add(entry_pointerEnter);
        eventTrigger.triggers.Add(entry_pointerExit);
    }

    void OnPointerEnter(PointerEventData eventData)
    {
        color.a = 1f;
        image.color = color;
    }

    void OnPointerExit(PointerEventData eventData)
    {
        color.a = 0f;
        image.color = color;
    }

    public abstract void OnPointerClick(PointerEventData eventData);
}
