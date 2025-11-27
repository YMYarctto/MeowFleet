using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.WSA;

public class ProbabilityMap
{
    ProbabilityMapDATA probability_map;

    LayoutDATA _layout;
    public List<LayoutDATA> layout_AllProbability
    {
        get => _layout.AllLayout();
    }

    public List<LayoutDATA> valid_layouts_inMap;

    public ProbabilityMap()
    {
        _layout = new();
        probability_map = new();
        valid_layouts_inMap = new();
    }

    public ProbabilityMap(LayoutDATA layout)
    {
        _layout = layout;
        probability_map = new();
        valid_layouts_inMap = new();
    }

    public void AddProbability(Vector2Int coord)
    {
        probability_map.AddProbability(coord);
    }

    public void AddProbability(Vector2Int center, LayoutDATA layout)
    {
        valid_layouts_inMap.Add(new(layout.LayoutInMap(center),layout.CoreNumber));
        probability_map.AddProbability(layout.LayoutInMap(center));
    }

    // 删除所有包含该点的概率布局
    public void DeleteProbability(Vector2Int center)
    {
        List<LayoutDATA> new_layouts = new(valid_layouts_inMap);
        foreach (var layout in new_layouts)
        {
            if (!layout.Contains(center))
            {
                continue;
            }
            valid_layouts_inMap.Remove(layout);
            probability_map.DeleteProbability(layout.ToList);
        }
    }

    // 删除该点该布局的概率布局
    public void DeleteProbabilityEach(Vector2Int center, LayoutDATA layout)
    {
        valid_layouts_inMap.Remove(layout);
        probability_map.DeleteProbability(layout.LayoutInMap(center));
    }

    // 删除不包含该点的所有概率布局
    public void DeleteProbabilityWithout(Vector2Int center)
    {
        List<LayoutDATA> new_layouts = new(valid_layouts_inMap);
        foreach (var layout in new_layouts)
        {
            if (layout.Contains(center))
            {
                continue;
            }
            valid_layouts_inMap.Remove(layout);
            probability_map.DeleteProbability(layout.ToList);
        }
    }
    
    public void RemoveProbability(Vector2Int center)
    {
        probability_map.RemoveProbability(center);
    }

    public Vector2Int GetHighProbabilityCoord()
    {
        // 按概率排序
        List<KeyValuePair<Vector2Int, int>> probability_map_list = probability_map.ToList().OrderByDescending(kv => kv.Value).ToList();
        List<Vector2Int> max_map = probability_map_list.Where(kv => kv.Value >= probability_map_list[0].Value).Select(kv=>kv.Key).ToList();

        // 随机选择一个最大概率点进行攻击
        System.Random rand = new();
        Vector2Int target = max_map[rand.Next(max_map.Count)];
        return target;
    }
    
    public string GetProbabilityMap()
    {
        return probability_map.ToString();
    }
}
