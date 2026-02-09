using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellGroup_Enemy : GridCellGroup_PVE
{
    public override UIView currentView => this;

    public static int GridCellID = 0;
    GameObject grid_cell;

    List<GridCell_Enemy> select_cells;

    public override void Init()
    {
        base.Init();
        GridCellID = 0;
        select_cells = new();
        mapSize = EnemyController.instance.size;

        grid_cell = ResourceManager.instance.GetPerfabByType<GridCell_PVE>();
        for (int i = 0; i < 100; i++)//TODO 100
        {
            GameObject obj = Instantiate(grid_cell);
            obj.transform.SetParent(transform, false);
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
                cell.Select(true);
            }
        }
    }

    public override void Hit(Vector2Int coord,bool isHit)
    {
        if (IsInMap(coord))
        {
            GridCell_Enemy cell = (GridCell_Enemy)girdCell_dict[GetIndex(coord)];
            cell.Hit(isHit);
        }
    }

    public void Check(Vector2Int coord)
    {
        if (IsInMap(coord))
        {
            GridCell_Enemy cell = (GridCell_Enemy)girdCell_dict[GetIndex(coord)];
            cell.Check();
        }
    }

    public void ClearSelect()
    {
        foreach (var cell in select_cells)
        {
            cell.Select(false);
        }
        select_cells.Clear();
    }
}
