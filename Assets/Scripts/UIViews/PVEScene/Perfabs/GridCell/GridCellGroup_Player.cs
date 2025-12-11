using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellGroup_Player : GridCellGroup_PVE
{
    public override UIView currentView => this;

    public static int GridCellID = 0;
    GameObject grid_cell;

    List<GridCell_Player> select_cells;

    public override void Init()
    {
        base.Init();
        select_cells = new();
        GridCellID = 0;
        grid_cell = ResourceManager.instance.GetPerfabByType<GridCell_PVE>();
        mapSize = DataManager.instance.SaveData.MapSize;
        for (int i = 0; i < 100; i++)//TODO 100
        {
            GameObject obj = Instantiate(grid_cell);
            obj.transform.SetParent(transform, false);
            obj.name = $"GridCell_{GridCellID}";
            girdCell_dict.Add(GridCellID, obj.AddComponent<GridCell_Player>());
            GridCellID++;
        }
    }

    public void Select(List<Vector2Int> coords)
    {
        foreach (var coord in coords)
        {
            if (IsInMap(coord))
            {
                GridCell_Player cell = (GridCell_Player)girdCell_dict[GetIndex(coord)];
                select_cells.Add(cell);
                cell.Select(true);
            }
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
