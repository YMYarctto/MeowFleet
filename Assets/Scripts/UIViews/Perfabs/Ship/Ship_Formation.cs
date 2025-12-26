using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Ship_Formation : Ship_UIBase
{
    Transform drag_parent => FormationController.instance.DragGroupTrans;
    Transform ship_parent=> FormationController.instance.ShipGroupTrans;
    Transform original_parent;

    public override UIView currentView => this;

    public Vector2 SizeDelta=>img_rect.sizeDelta;
    
    EventTrigger eventTrigger;

    GridCell_Formation drag_gridCell;
    bool isDragging = false;
    bool inMap = false;
    Vector2Int inMap_coord;
    Sequence loopTween;
    Vector3 pre_rotation;
    int direction;

    public override void Init()
    {
        loopTween = DOTween.Sequence();

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

    public override void Init(Ship ship)
    {
        base.Init(ship);
        original_parent = transform.parent;
        direction = 0;
        pre_rotation = trans.eulerAngles;
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
        if (DetectGridCell(eventData, out GridCell_Formation gridCell))
        {
            drag_gridCell = gridCell;
            FormationController.instance.SetDragLayout(gridCell.GetVector2Int(), ship.Layout);
        }

        direction = 0;
        pre_rotation = trans.eulerAngles;
    }

    private void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        MoveToMouse(eventData);

        if (DetectGridCell(eventData, out GridCell_Formation gridCell))
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
        if (DetectGridCell(eventData, out GridCell_Formation gridCell) && FormationController.instance.CanPlaced(gridCell.GetVector2Int(), ship.Layout))
        {
            pre_rotation = trans.eulerAngles;
            original_parent = gridCell.transform;
            trans.SetParent(original_parent, true);
            MoveAnimation(Vector2.zero, pre_rotation,ship_parent);
            inMap = true;
            inMap_coord = gridCell.GetVector2Int();
            Debug.Log($"检测到放置行为\n坐标: {inMap_coord}\n布局: {ship.Layout}");
        }
        else
        {
            trans.SetParent(original_parent, true);
            ship.Rotate(-direction);
            MoveAnimation(Vector2.zero, pre_rotation,original_parent);
        }
        if(inMap)
        {
            FormationController.instance.SetPlacedLayout(inMap_coord, ship.Layout,_ID);
        }
        FormationController.instance.ShipOnDrag = null;
        FormationController.instance.DragEnd();
    }

    private void MoveToMouse(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    private bool DetectGridCell(PointerEventData eventData, out GridCell_Formation gridCell)
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

    void MoveAnimation(Vector2 targetPos,Vector3 targetRot,Transform parent)
    {
        loopTween.Kill();
        loopTween = DOTween.Sequence();
        loopTween.SetUpdate(true);
        loopTween.Append(trans.DOLocalMove(targetPos, 0.1f).SetEase(Ease.OutQuad));
        loopTween.Join(trans.DORotate(targetRot, 0.1f).SetEase(Ease.OutQuad));
        loopTween.OnComplete(() => trans.SetParent(parent, true));
        loopTween.Play();
    }
}
