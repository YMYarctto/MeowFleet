using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseButton_TitleScene : BaseButton_Default
{
    TMP_Text text;

    public override void Init()
    {
        base.Init();

        text = GetComponentInChildren<TMP_Text>();
        text.color = PresetColor.Button_Idle;

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_pointerEnter = new EventTrigger.Entry();
        entry_pointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_pointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });

        EventTrigger.Entry entry_pointerExit = new EventTrigger.Entry();
        entry_pointerExit.eventID = EventTriggerType.PointerExit;
        entry_pointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });

        eventTrigger.triggers.Add(entry_pointerEnter);
        eventTrigger.triggers.Add(entry_pointerExit);
    }

    private void OnPointerEnter(PointerEventData eventData)
    {
        text.color = PresetColor.Button_Focus;
    }

    private void OnPointerExit(PointerEventData eventData)
    {
        text.color = PresetColor.Button_Idle;
    }
}
