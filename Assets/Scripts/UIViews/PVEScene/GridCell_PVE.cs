using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell_PVE : UIView
{
    public override UIView currentView => this;

    protected GameObject hit;
    protected GameObject select;

    int _ID = GridCellGroup_Player.GridCellID;
    public override int ID => _ID;

    public override void Init()
    {
        hit = transform.Find("hit").gameObject;
        select = transform.Find("select").gameObject;
        Disable();
    }

    public override void Enable()
    {
        hit.SetActive(true);
        select.SetActive(false);
    }

    public override void Disable()
    {
        hit.SetActive(false);
        select.SetActive(false);
    }

    public void Select()
    {
        hit.SetActive(false);
        select.SetActive(true);
    }

    public Vector2Int GetVector2Int()
    {
        return new(_ID % 10, _ID / 10);
    }
}