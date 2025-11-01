using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Ship_UI : UIView
{
    static Transform drag_parent;
    static Transform ship_parent;
    Transform original_parent;

    public override UIView currentView => this;
    public override int ID
    {
        get
        {
            _uid++;
            return _uid;
        }
    }

    static int _uid=10000;
    Ship current_ship;

    Transform trans;
    Image image;
    EventTrigger eventTrigger;

    public override void Init()
    {
        SetShip(Ships_Enum._1x3);//delete
        Task task = Task.Run(() =>
        {
            int loop_num = 0;
            while (current_ship == null)
            {
                loop_num++;
                Thread.Sleep(100);
                if(loop_num>50)
                {
                    Debug.LogError("无法获取舰船");
                    break;
                }
            }
        });
        task.Wait();

        trans = transform;
        drag_parent ??= GameObject.Find("OnDrag").transform;
        ship_parent ??= GameObject.Find("ShipGroup").transform;
        original_parent = transform.parent;

        // 调整大小
        image = transform.Find("sprite").GetComponent<Image>();
        Sprite sprite = ResourceManager.instance.GetSprite(current_ship.Name);
        image.sprite = sprite;
        RectTransform rectTransform = image.rectTransform;
        Vector2 spriteSize = new Vector2(sprite.rect.width, sprite.rect.height);
        float pixelsPerUnit = sprite.pixelsPerUnit;
        float scale = 1f;
        if (image.canvas)scale = image.canvas.scaleFactor;
        rectTransform.sizeDelta = spriteSize / pixelsPerUnit * scale;
        rectTransform.pivot = sprite.pivot;
        rectTransform.localPosition = Vector2.zero;

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_pointerClick = new EventTrigger.Entry();
        entry_pointerClick.eventID = EventTriggerType.PointerClick;
        entry_pointerClick.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });

        EventTrigger.Entry entry_onDrag = new EventTrigger.Entry();
        entry_onDrag.eventID = EventTriggerType.Drag;
        entry_onDrag.callback.AddListener((data) => { OnDragBegin((PointerEventData)data); });

        EventTrigger.Entry entry_beginDrag = new EventTrigger.Entry();
        entry_beginDrag.eventID = EventTriggerType.BeginDrag;
        entry_beginDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        
        EventTrigger.Entry entry_endDrag = new EventTrigger.Entry();
        entry_endDrag.eventID = EventTriggerType.EndDrag;
        entry_endDrag.callback.AddListener((data) => { OnDragEnd((PointerEventData)data); });

        eventTrigger.triggers.Add(entry_pointerClick);
        eventTrigger.triggers.Add(entry_beginDrag);
        eventTrigger.triggers.Add(entry_onDrag);
        eventTrigger.triggers.Add(entry_endDrag);
    }

    private void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click");
    }

    private void OnDragBegin(PointerEventData eventData)
    {
        trans.SetParent(drag_parent,true);
        trans.position = eventData.position;
    }

    private void OnDrag(PointerEventData eventData)
    {
        trans.position = eventData.position;
    }

    private void OnDragEnd(PointerEventData eventData)
    {
        // 检测当前鼠标下是否有格子
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("GridCell"))
        {
            original_parent = eventData.pointerEnter.transform;
            transform.SetParent(original_parent, true);
            trans.localPosition = Vector2.zero;
            transform.SetParent(original_parent, true);
        }
        else
        {
            transform.SetParent(original_parent, false);
            trans.localPosition = Vector2.zero;
        }
    }

    public void SetShip(Ships_Enum id)
    {
        current_ship = new(DataManager.instance.GetShipData(id));
    }
    
    
}
