using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;

public class Ship_UI : UIView
{
    static Transform drag_parent;
    static Transform ship_parent;
    Transform original_parent;

    static Canvas canvas;

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
    RectTransform rectTransform;
    Image image;
    EventTrigger eventTrigger;

    bool isDragging = false;

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
                if (loop_num > 50)
                {
                    Debug.LogError("无法获取舰船");
                    break;
                }
            }
        });
        task.Wait();

        trans = transform;
        rectTransform = trans.GetComponent<RectTransform>();
        drag_parent ??= GameObject.Find("OnDrag").transform;
        ship_parent ??= GameObject.Find("ShipGroup").transform;
        original_parent = transform.parent;

        canvas ??= GameObject.Find("Canvas_FormationScene").GetComponent<Canvas>();

        // 调整大小
        image = transform.Find("sprite").GetComponent<Image>();
        Sprite sprite = ResourceManager.instance.GetSprite(current_ship.Name);
        image.sprite = sprite;
        RectTransform img_rectTransform = image.rectTransform;
        Vector2 spriteSize = new Vector2(sprite.rect.width, sprite.rect.height);
        float pixelsPerUnit = sprite.pixelsPerUnit;
        float scale = 1f;
        if (image.canvas) scale = image.canvas.scaleFactor;
        img_rectTransform.sizeDelta = spriteSize / pixelsPerUnit * scale;
        img_rectTransform.pivot = new Vector2(
            sprite.pivot.x / sprite.rect.width,
            sprite.pivot.y / sprite.rect.height
        );
        img_rectTransform.localPosition = Vector2.zero;

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_pointerUp = new EventTrigger.Entry();
        entry_pointerUp.eventID = EventTriggerType.PointerUp;
        entry_pointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });

        EventTrigger.Entry entry_pointerDown = new EventTrigger.Entry();
        entry_pointerDown.eventID = EventTriggerType.PointerDown;
        entry_pointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });

        EventTrigger.Entry entry_onDrag = new EventTrigger.Entry();
        entry_onDrag.eventID = EventTriggerType.Drag;
        entry_onDrag.callback.AddListener((data) => { OnDragBegin((PointerEventData)data); });

        EventTrigger.Entry entry_beginDrag = new EventTrigger.Entry();
        entry_beginDrag.eventID = EventTriggerType.BeginDrag;
        entry_beginDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });

        EventTrigger.Entry entry_endDrag = new EventTrigger.Entry();
        entry_endDrag.eventID = EventTriggerType.EndDrag;
        entry_endDrag.callback.AddListener((data) => { OnDragEnd((PointerEventData)data); });

        eventTrigger.triggers.Add(entry_pointerUp);
        eventTrigger.triggers.Add(entry_pointerDown);
        eventTrigger.triggers.Add(entry_beginDrag);
        eventTrigger.triggers.Add(entry_onDrag);
        eventTrigger.triggers.Add(entry_endDrag);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        if(!isDragging)
        {
            Debug.Log("click");
        }
    }

    private void OnDragBegin(PointerEventData eventData)
    {
        trans.SetParent(drag_parent,true);
        MoveToMouse(eventData);
    }

    private void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        MoveToMouse(eventData);

        if(DetectGridCell(eventData,out GridCell gridCell))
        {
            gridCell.Allow(true);
        }
    }

    private void OnDragEnd(PointerEventData eventData)
    {
        // 检测当前鼠标下是否有格子
        if (DetectGridCell(eventData,out GridCell gridCell))
        {
            original_parent = ship_parent;
            transform.SetParent(gridCell.transform, true);
            trans.localPosition = Vector2.zero;
            transform.SetParent(original_parent, true);
        }
        else
        {
            transform.SetParent(original_parent, true);
            trans.localPosition = Vector2.zero;
        }
    }

    private void MoveToMouse(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPos);
        rectTransform.anchoredPosition = localPos;
    }

    private bool DetectGridCell(PointerEventData eventData,out GridCell gridCell)
    {
        gridCell = null;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            if (r.gameObject.CompareTag("GridCell"))
            {
                gridCell = r.gameObject.GetComponent<GridCell_raycast>().Parent;
                return true;
            }
        }
        return false;
    }

    public void SetShip(Ships_Enum id)
    {
        current_ship = new(DataManager.instance.GetShipData(id));
    }
    
}
