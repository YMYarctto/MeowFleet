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
using DG.Tweening;

public class Ship_UI : UIView
{
    Transform drag_parent => FormationController.instance.DragGroupTrans;
    Transform ship_parent=> FormationController.instance.ShipGroupTrans;
    Transform original_parent;

    Canvas canvas => FormationController.instance.canvas;

    public override UIView currentView => this;

    int _ID = id;
    public override int ID => _ID;

    static int id
    {
        get
        {
            _uid++;
            return _uid;
        }
    }

    static int _uid=10000;
    Ship ship;

    Transform trans;
    RectTransform rectTransform;
    Image image;
    EventTrigger eventTrigger;

    GridCell drag_gridCell;
    bool isDragging = false;
    bool inMap = false;
    Vector2Int inMap_coord;

    public override void Init()
    {
        SetShip(1);//delete
        Task task = Task.Run(() =>
        {
            int loop_num = 0;
            while (ship == null)
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
        original_parent = transform.parent;

        // 调整大小
        image = trans.Find("sprite").GetComponent<Image>();
        Sprite sprite = ResourceManager.instance.GetSprite(ship.Uid);
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
        entry_onDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });

        EventTrigger.Entry entry_beginDrag = new EventTrigger.Entry();
        entry_beginDrag.eventID = EventTriggerType.BeginDrag;
        entry_beginDrag.callback.AddListener((data) => { OnDragBegin((PointerEventData)data); });

        EventTrigger.Entry entry_endDrag = new EventTrigger.Entry();
        entry_endDrag.eventID = EventTriggerType.EndDrag;
        entry_endDrag.callback.AddListener((data) => { OnDragEnd((PointerEventData)data); });

        eventTrigger.triggers.Add(entry_pointerUp);
        eventTrigger.triggers.Add(entry_pointerDown);
        eventTrigger.triggers.Add(entry_beginDrag);
        eventTrigger.triggers.Add(entry_onDrag);
        eventTrigger.triggers.Add(entry_endDrag);
    }

    public void Init(int id)
    {
        SetShip(id);
        trans = transform;
        trans.localPosition = Vector2.zero;
        rectTransform = trans.GetComponent<RectTransform>();
        original_parent = transform.parent;

        // 调整大小
        image = trans.Find("sprite").GetComponent<Image>();
        Sprite sprite = ResourceManager.instance.GetSprite(ship.Uid);
        image.sprite = sprite;
        RectTransform img_rectTransform = image.rectTransform;
        Vector2 spriteSize = new Vector2(sprite.rect.width, sprite.rect.height);
        float pixelsPerUnit = sprite.pixelsPerUnit;
        img_rectTransform.sizeDelta = spriteSize / pixelsPerUnit;
        img_rectTransform.pivot = new Vector2(
            sprite.pivot.x / sprite.rect.width,
            sprite.pivot.y / sprite.rect.height
        );
        img_rectTransform.localPosition = Vector2.zero;
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

        if (inMap)
        {
            FormationController.instance.RemovePlaced(inMap_coord);
        }
        
        FormationController.instance.ShipOnDrag = this;
        FormationController.instance.DragBegin();
        if (DetectGridCell(eventData, out GridCell gridCell))
        {
            drag_gridCell = gridCell;
            FormationController.instance.SetDragLayout(gridCell.GetVector2Int(), ship.Layout);
        }
    }

    private void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        MoveToMouse(eventData);

        if (DetectGridCell(eventData, out GridCell gridCell))
        {
            if (drag_gridCell != gridCell)
            {
                drag_gridCell = gridCell;
                FormationController.instance.SetDragLayout(gridCell.GetVector2Int(), ship.Layout);
            }
        }
        else if (drag_gridCell != null)
        {
            FormationController.instance.RefreshDrag();
            drag_gridCell = null;
        }
    }

    private void OnDragEnd(PointerEventData eventData)
    {
        // 检测当前鼠标下是否有格子
        if (DetectGridCell(eventData, out GridCell gridCell) && FormationController.instance.CanPlaced(gridCell.GetVector2Int(), ship.Layout))
        {
            original_parent = gridCell.transform;
            trans.SetParent(original_parent, true);
            MoveAnimation(Vector2.zero);
            inMap = true;
            inMap_coord = gridCell.GetVector2Int();
            FormationController.instance.SetPlacedLayout(inMap_coord, ship.Layout);
        }
        else
        {
            trans.SetParent(original_parent, true);
            MoveAnimation(Vector2.zero);
        }
        FormationController.instance.ShipOnDrag = null;
        FormationController.instance.DragEnd();
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
    
    public void Rotate(int direction)
    {
        ship.Rotate(direction);
        trans.eulerAngles += new Vector3(0, 0, -direction * 90);
        if(drag_gridCell!=null)FormationController.instance.SetDragLayout(drag_gridCell.GetVector2Int(),ship.Layout);
    }

    void SetShip(int id)
    {
        ship = new(DataManager.instance.GetShipData(id));
    }

    void MoveAnimation(Vector2 targetPos)
    {
        trans.DOKill();
        trans.DOLocalMove(targetPos, 0.1f).SetUpdate(true).SetEase(Ease.OutQuad).OnComplete(()=>trans.SetParent(ship_parent, true));
    }

    public static Ship_UI Create(GameObject prefab, int id)
    {
        var obj = Instantiate(prefab);
        var ui = obj.GetComponent<Ship_UI>();
        ui.Init(id);
        return ui;
    }
}
