using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridCellGroup_Enemy : GridCellGroup_PVE
{
    public override UIView currentView => this;

    public static int GridCellID = 0;
    GameObject grid_cell;

    List<GridCell_Enemy> select_cells;

    public override void Init()
    {
        base.Init();
        select_cells = new();
        mapSize = EnemyController.instance.size;

        grid_cell = ResourceManager.instance.GetPerfabByType<GridCell_PVE>();
        for (int i = 0; i < 100; i++)//TODO 100
        {
            GameObject obj = Instantiate(grid_cell);
            obj.transform.SetParent(transform);
            obj.name = $"GridCell_{GridCellID}";
            girdCell_dict.Add(GridCellID, obj.AddComponent<GridCell_Enemy>());
            GridCellID++;
        }
    }

    public void Select(List<Vector2Int> coords)
    {
        foreach (var coord in coords)
        {
            if (IsInMap(coord))
            {
                GridCell_Enemy cell = (GridCell_Enemy)girdCell_dict[GetIndex(coord)];
                select_cells.Add(cell);
                cell.Select();
            }
        }
    }

    public void ClearSelect()
    {
        foreach (var cell in select_cells)
        {
            cell.Disable();
        }
        select_cells.Clear();
    }
    
    private bool IsInMap(Vector2Int v2)
    {
        return v2.x >= 0 && v2.x < mapSize.x && v2.y >= 0 && v2.y < mapSize.y;
    }
}
