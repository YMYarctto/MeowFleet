using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellGroup : UIView
{
    public override UIView currentView => this;

    public static int GridCellID = 0;

    Transform trans;
    GameObject grid_cell;

    public override void Init()
    {
        trans = transform;
        grid_cell = ResourceManager.instance.GetPerfabByType<GridCell>();
        for(int i=0;i<100;i++)//TODO 100
        {
            GameObject obj = Instantiate(grid_cell);
            obj.transform.SetParent(trans);
            obj.name = $"GridCell_{GridCellID}";
            GridCellID++;
        }
    }

}
