using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InformationCard_UI : UIView
{
    public override UIView currentView => this;
    public override int ID => _id;

    int _id = InformationBoard.CardID;

    TMP_Text title;
    TMP_Text content;

    EventTrigger eventTrigger;

    public override void Init()
    {
        Transform inner = transform.Find("inner");
        title = inner.Find("title").GetComponent<TMP_Text>();
        content = inner.Find("content").GetComponent<TMP_Text>();
        title.text = UIManager.instance.GetUIView<InformationBoard>().GetTitle();

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

    public override void Enable()
    {
        transform.DOLocalMoveX(-295,0.3f).SetEase(Ease.OutQuad);
    }

    public InformationCard_UI Hit(string ship_str, string locate)
    {
        content.text = $"你命中了<link=\"id\"><color=#b51d04>【 {ship_str} 】</color></link>的{locate}";
        return this;
    }
    
    public InformationCard_UI Destroy(string ship_str,string action)
    {
        content.text = $"你{action}了<link=\"id\"><color=#b51d04>【 {ship_str} 】</color></link>";
        return this;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<InformationBoard>().OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<InformationBoard>().OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<InformationBoard>().OnDrag(eventData);
    }
}
