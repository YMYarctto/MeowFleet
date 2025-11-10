using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridCellGroup_PVE : UIView
{
    public static int GridCellID = 0;
    protected Dictionary<int, GridCell_PVE> girdCell_dict;

    public override void Init()
    {
        girdCell_dict = new();
    }
    
    public void Hit()
    {
        
    }
}
