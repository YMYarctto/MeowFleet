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
        ui.gameObject.SetActive(false);
        trans.eulerAngles += new Vector3(0, 0, -ship.Direction * 90);
    }
    
    public void SetPosition(Vector2Int coord)
    {
        Transform init_parent = transform.parent;
        transform.SetParent(UIManager.instance.GetUIView<GridCellGroup_Player>().GetGridCell(coord).transform, true);
        transform.localPosition = Vector3.zero;
        transform.SetParent(init_parent, true);
    }
}
