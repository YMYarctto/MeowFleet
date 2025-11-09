using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridCellGroup_Formation : UIView
{
    public override UIView currentView => this;

    public static int GridCellID = 0;

    Transform trans;
    GameObject grid_cell;

    Dictionary<int, GridCell_Formation> girdCell_dict;
    List<GridCell_Formation> current_drag;
    List<GridCell_Formation> placed;

    public override void Init()
    {
        GridCellID = 0;
        girdCell_dict = new();
        current_drag = new();
        placed = new();

        trans = transform;
        grid_cell = ResourceManager.instance.GetPerfabByType<GridCell_Formation>();
        for (int i = 0; i < 100; i++)//TODO 100
        {
            GameObject obj = Instantiate(grid_cell);
            obj.transform.SetParent(trans);
            obj.name = $"GridCell_{GridCellID}";
            girdCell_dict.Add(GridCellID, obj.GetComponent<GridCell_Formation>());
            GridCellID++;
        }
    }

    public void RefreshDrag()
    {
        foreach (var current in current_drag)
        {
            if (placed.Contains(current))
            {
                current.Allow(true);
            }
            else
            {
                current.Disable();
            }
        }
        current_drag = new();
    }

    public void RefreshAll()
    {
        RefreshDrag();
        foreach (var gridCell in placed)
        {
            gridCell.Disable();
        }
        placed = new();
    }

    public void SetPlaced(List<Vector2Int> coords)
    {
        foreach (var coord in coords)
        {
            GridCell_Formation current = girdCell_dict[GetIndex(coord)];
            current.Allow(true);
            placed.Add(current);
        }
    }

    public void SetDragAllow(Vector2Int coord, bool isAllow)
    {
        GridCell_Formation current = girdCell_dict[GetIndex(coord)];
        current.Allow(isAllow);
        current_drag.Add(current);
    }

    int GetIndex(Vector2Int v2)
    {
        return v2.y * 10 + v2.x;
    }
}
