using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridCellGroup_PVE : UIView
{
    protected Dictionary<int, GridCell_PVE> girdCell_dict;

    protected Vector2Int mapSize;

    public override void Init()
    {
        girdCell_dict = new();
    }

    public void Hit(Vector2Int v2)
    {
        int index = GetIndex(v2);
        girdCell_dict[index].Enable();
    }

    public GridCell_PVE GetGridCell(Vector2Int coord)
    {
        return girdCell_dict[GetIndex(coord)];
    }

    protected int GetIndex(Vector2Int v2)
    {
        return v2.y * mapSize.x + v2.x;
    }
}
