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
        {typeof(InformationWindow_UI),"InformationWindow"},
        {typeof(FX_bomb2),"FX_bomb2"},
        {typeof(Block),"block"},
        {typeof(ShipContainer),"ShipContainer"},
        {typeof(FX_holeWriter),"FX_holeWriter"},
    };

    public static Dictionary<Type,string> skill_card_sprite = new()
    {
        {typeof(bomb_focus),"bomb_focus"},
        {typeof(bomb_wide),"bomb_wide"},
        {typeof(torpedo),"torpedo"},
        {typeof(radar),"radar"},
    };

    public static Dictionary<string, Vector2> ships_sprite_pivot = new()
    {
        {"radar#0_AB0",new(0.5f,0.5f)},
        {"bomb_focus#2_AB0",new(0.5f,0.5f)},
        {"bomb_wide#2_B1",new(0.5f,0.5f)},
        {"torpedo#4_A0",new(0.75f,0.25f)},
    };
}
