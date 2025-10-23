using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ProbabilityMap
{
    ProbabilityMapDATA probability_map;

    LayoutDATA _layout;
    public LayoutDATA layout_mirror
    {
        get => _layout.Mirror();
    }

    public List<LayoutDATA> valid_layouts;

    public ProbabilityMap()
    {
        _layout = new();
        probability_map = new();
        valid_layouts = new();
    }

    public ProbabilityMap(LayoutDATA layout)
    {
        _layout = layout;
        probability_map = new();
        valid_layouts = new();
    }

    public void AddProbability(Vector2Int coord)
    {
        probability_map.AddProbability(coord);
    }

    public void AddProbability(Vector2Int center, LayoutDATA layout)
    {
        valid_layouts.Add(layout);
        probability_map.AddProbability(layout.Current.ConvertAll(v => center + v));
    }

    public void DeleteProbability(Vector2Int center, Converter<LayoutDATA, bool> CheckLayoutValid)
    {
        foreach (var layout in valid_layouts)
        {
            if (!CheckLayoutValid.Invoke(layout))
            {
                continue;
            }
            valid_layouts.Remove(layout);
            probability_map.DeleteProbability(layout.Current.ConvertAll(v => center + v));
        }
    }
    
    public void DeleteProbabilityEach(Vector2Int center,LayoutDATA layout)
    {
        valid_layouts.Remove(layout);
        probability_map.DeleteProbability(layout.Current.ConvertAll(v => center + v));
    }
    
    public Vector2Int GetHighProbabilityCoord()
    {
        // 按概率排序
        List<KeyValuePair<Vector2Int, int>> probability_map_list = probability_map.ToList();
        probability_map_list.OrderByDescending(kv => kv.Value);

        // 随机选择一个较大概率点进行攻击
        System.Random rand = new();
        Vector2Int target = probability_map_list[rand.Next(probability_map_list.Count / 4)].Key;
        return target;
    }
}
