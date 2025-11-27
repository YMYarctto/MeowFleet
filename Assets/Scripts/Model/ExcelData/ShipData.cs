using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShipData
{
    public int uid;
    public int pre_id;
    public string brench;
    public int level;
    public string skill_name_string;
    public string ship_name_string;
    public string shape_coord_string;
    public int core_number;
    public string skill_core_string;

    public Ships_Enum ship_name;
    public List<Vector2Int> shape_coord;
    public string Url=> $"{ship_name}_{level}";
}
