using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfomationRollBar : UIView
{
    public override UIView currentView => this;

    InformationBoard informationBoard;

    float total_length;
    Vector3 last_pos;

    RectTransform viewport;
    RectTransform bar;

    EventTrigger eventTrigger;

    public override void Init()
    {
        viewport = transform.GetComponent<RectTransform>();
        bar = transform.Find("bar").GetComponent<RectTransform>();
        total_length = viewport.rect.height;

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_beginDrag = new EventTrigger.Entry();
        entry_beginDrag.eventID = EventTriggerType.BeginDrag;
        entry_beginDrag.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });

        EventTrigger.Entry entry_endDrag = new EventTrigger.Entry();
        entry_endDrag.eventID = EventTriggerType.EndDrag;
        entry_endDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });

        EventTrigger.Entry entry_onDrag = new EventTrigger.Entry();
        entry_onDrag.eventID = EventTriggerType.Drag;
        entry_onDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });

        eventTrigger.triggers.Add(entry_beginDrag);
        eventTrigger.triggers.Add(entry_endDrag);
        eventTrigger.triggers.Add(entry_onDrag);
    }

    void Start()
    {
        informationBoard = UIManager.instance.GetUIView<InformationBoard>();
    }

    public void Set()
    {
        if(informationBoard.UnviewLength<=0)
        {
            bar.anchoredPosition = new Vector2(bar.anchoredPosition.x, 0);
            bar.sizeDelta = new Vector2(bar.sizeDelta.x, total_length);
            return;
        }
        bar.sizeDelta = new Vector2(bar.sizeDelta.x, total_length * (informationBoard.ViewLength / informationBoard.TotalLength));
        bar.anchoredPosition = new Vector2(bar.anchoredPosition.x, - (total_length - bar.sizeDelta.y) * (informationBoard.CurrentLength / informationBoard.UnviewLength));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        last_pos = eventData.position;
        SetBoard(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetBoard(eventData);
        last_pos = eventData.position;
    }

    void SetBoard(PointerEventData eventData)
    {
        float deltaY = eventData.position.y - last_pos.y;
        float archoredY = Mathf.Clamp(bar.anchoredPosition.y+deltaY,-(total_length-bar.sizeDelta.y),0);
        bar.anchoredPosition = new Vector2(bar.anchoredPosition.x, archoredY);
        informationBoard.Set(total_length-bar.sizeDelta.y<=0?0:-archoredY / (total_length - bar.sizeDelta.y));
    }
}
