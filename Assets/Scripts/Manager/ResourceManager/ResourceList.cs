using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceList
{
    public static Dictionary<Type, string> gameobjects = new()
    {
        {typeof(GridCell_Formation),"GridCell_Formation"},
        {typeof(GridCell_PVE),"GridCell_PVE"},
        {typeof(Ship_UIBase),"Ship"},
        {typeof(Pointer_Animation),"Pointer"},
        {typeof(PVE_Notice),"Notice"},
        {typeof(SkillCard_UI),"SkillCard"},
        {typeof(FX_bomb),"FX_bomb"},
        {typeof(InformationCard_UI),"InformationCard"},
        {typeof(FX_bomb2),"FX_bomb2"},
    };

    public static Dictionary<string, Vector2> ships_sprite_pivot = new()
    {
        {"_1x2_0",new(0.75f,0.25f)},
        {"_1x3_0",new(0.5f,0.5f)},
        {"_3a2_0",new(0.5f,0.5f)},
        {"_3x3_0",new(0.5f,0.5f)},
    };
}
