using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship_PVE : Ship_UIBase
{
    public override void Init()
    {

    }

    public override void Init(Ship ship)
    {
        base.Init(ship);
        trans.SetParent(PVEController.instance.ShipGroupTrans,true);
    }
}
