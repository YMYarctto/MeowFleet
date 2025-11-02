using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceList
{
    public static Dictionary<Type, string> gameobjects = new()
    {
        {typeof(GridCell),"GridCell"},
    };

    public static Dictionary<Ships_Enum, Vector2> ships_sprite_pivot = new()
    {
        {Ships_Enum._1x2,new(0.5f,0.5f)},
        {Ships_Enum._1x3,new(0.5f,0.5f)},
    };
}
