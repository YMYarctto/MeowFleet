using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship_PVE : Ship_UIBase
{
    public override void Init()
    {

    }
    
    public void SetPosition(Vector2Int coord)
    {
        Transform init_parent = transform.parent;
        transform.SetParent(UIManager.instance.GetUIView<GridCellGroup_Player>().GetGridCell(coord).transform, false);
        transform.localPosition = Vector3.zero;
        transform.SetParent(init_parent, true);
    }
}
