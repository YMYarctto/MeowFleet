using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridCell_PVE : UIView
{
    protected GameObject hit;
    protected GameObject select;

    public override void Init()
    {
        hit = transform.Find("hit").gameObject;
        select = transform.Find("select").gameObject;
        Disable();
    }

    public override void Enable()
    {
        hit.SetActive(true);
    }

    public override void Disable()
    {
        hit.SetActive(false);
        select.SetActive(false);
    }

    public void Select(bool is_select)
    {
        select.SetActive(is_select);
    }

    public Vector2Int GetVector2Int()
    {
        return new(ID % 10, ID / 10);
    }
}