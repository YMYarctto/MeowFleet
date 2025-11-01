using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : UIView
{
    public override UIView currentView => this;

    GameObject allow;
    GameObject forbid;

    public override int ID => GridCellGroup.GridCellID;

    public override void Init()
    {
        allow = transform.Find("allow").gameObject;
        forbid = transform.Find("forbid").gameObject;
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
