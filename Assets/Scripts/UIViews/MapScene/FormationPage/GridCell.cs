using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : UIView
{
    public override UIView currentView => this;

    GameObject allow;
    GameObject forbid;
    Transform raycast;

    static Transform raycast_group;

    public override int ID => GridCellGroup.GridCellID;

    public override void Init()
    {
        allow = transform.Find("allow").gameObject;
        forbid = transform.Find("forbid").gameObject;
        raycast = transform.Find("raycast");
        raycast.gameObject.name = $"raycast_{ID}";
        raycast_group ??= GameObject.Find("RaycastGroup").transform;
        raycast.SetParent(raycast_group, true);
        raycast.GetComponent<GridCell_raycast>().Parent = this;
        Disable();
    }

    public void Allow(bool isAllow)
    {
        allow.SetActive(isAllow);
        forbid.SetActive(isAllow);
    }

    public override void Disable()
    {
        allow.SetActive(false);
        forbid.SetActive(false);
    }

}
