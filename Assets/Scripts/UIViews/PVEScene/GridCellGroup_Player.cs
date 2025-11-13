using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellGroup_Player : GridCellGroup_PVE
{
    public override UIView currentView => this;

    public static int GridCellID = 0;
    GameObject grid_cell;

    public override void Init()
    {
        base.Init();
        grid_cell = ResourceManager.instance.GetPerfabByType<GridCell_PVE>();
        mapSize = DataManager.instance.SaveData.MapSize;
        for (int i = 0; i < 100; i++)//TODO 100
        {
            GameObject obj = Instantiate(grid_cell);
            obj.transform.SetParent(transform,false);
            obj.name = $"GridCell_{GridCellID}";
            girdCell_dict.Add(GridCellID, obj.AddComponent<GridCell_PVE>());
            GridCellID++;
        }
    }
    
}
