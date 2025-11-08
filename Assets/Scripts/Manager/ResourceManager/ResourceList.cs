using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceList
{
    public static Dictionary<Type, string> gameobjects = new()
    {
        {typeof(GridCell),"GridCell"},
        {typeof(Ship_UI),"Ship_Formation"},
        {typeof(Pointer_Animation),"Pointer"},
    };

    public static Dictionary<string, Vector2> ships_sprite_pivot = new()
    {
        {"_1x2_0",new(0.75f,0.25f)},
        {"_1x3_0",new(0.5f,0.5f)},
        {"_3a2_0",new(0.5f,0.5f)},
        {"_3x3_0",new(0.5f,0.5f)},
    };
}
