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
}
