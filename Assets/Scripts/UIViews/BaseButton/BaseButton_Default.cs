using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseButton_Default : UIView
{
    protected EventTrigger eventTrigger;

    private float darkAlpha = 0.6f;
    private float fadeDuration = 0.15f;

    private Image image;
    private Color color;
    private Tween tween;

    public override void Init()
    {
        image = GetComponent<Image>();
        color = image.color;

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_pointerDown = new EventTrigger.Entry();
        entry_pointerDown.eventID = EventTriggerType.PointerDown;
        entry_pointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });

        EventTrigger.Entry entry_pointerUp = new EventTrigger.Entry();
        entry_pointerUp.eventID = EventTriggerType.PointerUp;
        entry_pointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });

        eventTrigger.triggers.Add(entry_pointerDown);
        eventTrigger.triggers.Add(entry_pointerUp);
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        tween?.Kill();
        tween = image.DOColor(color, fadeDuration);
    }

    private void OnPointerDown(PointerEventData eventData)
    {
        tween?.Kill();
        Color darkColor = color * darkAlpha;
        darkColor.a = color.a;
        tween = image.DOColor(darkColor, fadeDuration);
    }
}
