using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : UIView
{
    public override UIView currentView => this;

    GameObject allow;
    GameObject forbid;
    Transform raycast;

    Transform raycast_group=>FormationController.instance.RaycastGroup;

    int _ID = GridCellGroup.GridCellID;
    public override int ID => _ID;

    public override void Init()
    {
        allow = transform.Find("allow").gameObject;
        forbid = transform.Find("forbid").gameObject;
        raycast = transform.Find("raycast");
        raycast.gameObject.name = $"raycast_{_ID}";
        raycast.SetParent(raycast_group, true);
        raycast.GetComponent<GridCell_raycast>().Parent = this;
        Disable();
    }

    void Start()
    {
        raycast.SetParent(raycast_group, true);
    }

    public void Allow(bool isAllow)
    {
        allow.SetActive(isAllow);
        forbid.SetActive(!isAllow);
    }

    public override void Disable()
    {
        allow.SetActive(false);
        forbid.SetActive(false);
    }

    public Vector2Int GetVector2Int()
    {
        return new(_ID % 10, _ID / 10);
    }
}
