using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridCell_PVE : UIView
{
    protected GameObject hit_on;
    protected GameObject hit_null;
    protected GameObject select;

    public override void Init()
    {
        hit_on = transform.Find("hit_on").gameObject;
        hit_null = transform.Find("hit_null").gameObject;
        select = transform.Find("select").gameObject;
        Disable();
    }

    public void Hit(bool isHit)
    {
        hit_on.SetActive(isHit);
        hit_null.SetActive(!isHit);
    }

    public override void Disable()
    {
        hit_on.SetActive(false);
        hit_null.SetActive(false);
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