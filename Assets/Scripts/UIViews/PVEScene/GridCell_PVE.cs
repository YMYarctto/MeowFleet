using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell_PVE : UIView
{
    public override UIView currentView => this;

    GameObject hit;
    GameObject raycast;

    int _ID = GridCellGroup_PVE.GridCellID;
    public override int ID => _ID;

    public override void Init()
    {
        hit = transform.Find("hit").gameObject;
        raycast = transform.Find("raycast").gameObject;
        Disable();
    }

    public override void Enable()
    {
        hit.SetActive(true);
    }

    public override void Disable()
    {
        hit.SetActive(false);
    }

    public Vector2Int GetVector2Int()
    {
        int id = _ID;
        while (id >= 1000)
        {
            id -= 1000;
        }
        return new(id % 10, id / 10);
    }
}