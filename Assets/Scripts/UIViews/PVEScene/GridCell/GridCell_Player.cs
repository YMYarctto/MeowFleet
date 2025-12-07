using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridCell_Player : GridCell_PVE
{
    public override UIView currentView => this;

    int _ID = GridCellGroup_Player.GridCellID;
    public override int ID => _ID;

    EventTrigger eventTrigger;

    public override void Init()
    {
        base.Init();
        eventTrigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry_pointerClick = new EventTrigger.Entry();
        entry_pointerClick.eventID = EventTriggerType.PointerClick;
        entry_pointerClick.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_pointerClick);

        EventTrigger.Entry entry_pointerEnter = new EventTrigger.Entry();
        entry_pointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_pointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_pointerEnter);

        EventTrigger.Entry entry_pointerExit = new EventTrigger.Entry();
        entry_pointerExit.eventID = EventTriggerType.PointerExit;
        entry_pointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_pointerExit);

    }

    private void OnPointerEnter(PointerEventData data)
    {
        PVEController.instance.PlayerSelect(GetVector2Int(),PVEController.PVEMap.Player);
        PVEController.instance.Aim(PVEController.PVEMap.Player,transform.position);
    }

    private void OnPointerExit(PointerEventData data)
    {
        PVEController.instance.ClearSelect();
        PVEController.instance.Aim(false);
    }

    private void OnPointerClick(PointerEventData data)
    {
        PVEController.instance.PlayerOP(GetVector2Int(),PVEController.PVEMap.Player);
        PVEController.instance.Aim(false);
    }
}
