using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SafeArea : UIView
{
    public override UIView currentView => this;

    Transform OnDrag;
    bool isDrag => OnDrag.childCount==0;
    List<Transform> containers;

    Transform focus;
    TMP_Text count;

    public override void Init()
    {
        OnDrag = GameObject.Find("OnDrag").transform;
        focus = transform.Find("focus");
        focus.gameObject.SetActive(false);
        count = transform.Find("count").GetComponent<TMP_Text>();
        var transforms = transform.GetComponentsInChildren<Transform>(true);
        containers = new();
        foreach (var trans in transforms)
        {
            if (trans.CompareTag("SafeAreaContainer"))
            {
                containers.Add(trans);

                // EventTrigger eventTrigger = trans.AddComponent<EventTrigger>();

                // EventTrigger.Entry entry_pointerEnter = new EventTrigger.Entry();
                // entry_pointerEnter.eventID = EventTriggerType.PointerEnter;
                // entry_pointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data,trans.transform); });

                // EventTrigger.Entry entry_pointerExit = new EventTrigger.Entry();
                // entry_pointerExit.eventID = EventTriggerType.PointerExit;
                // entry_pointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });

                // eventTrigger.triggers.Add(entry_pointerExit);
                // eventTrigger.triggers.Add(entry_pointerEnter);
            }
        }
    }

    // private void OnPointerExit(PointerEventData data)
    // {
    //     focus.gameObject.SetActive(false);
    // }

    // private void OnPointerEnter(PointerEventData data,Transform target)
    // {
    //     if(target.childCount!=0^isDrag)
    //     {
    //         return;
    //     }
    //     focus.position = target.transform.position;
    //     focus.gameObject.SetActive(true);
    // }

    public void UpdateCount()
    {
        count.text = $"{containers.Count(c=>c.childCount==0)}";
    }
}
