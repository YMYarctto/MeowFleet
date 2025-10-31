using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShipData
{
    public int uid;
    public int pro_id;
    public int level;
    public string ship_name_string;
    public string shape_coord_string;
    public Ships_Enum ship_name;
    public List<Vector2Int> shape_coord;
}
