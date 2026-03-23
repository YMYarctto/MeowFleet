using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        LayoutDATA absoluteLayout = new(layout.LayoutInMap(center), layout.CoreNumber);
        if (!TryRemoveLayout(absoluteLayout, out var removedLayout))
        {
            return;
        }

        probability_map.DeleteProbability(removedLayout.ToList);
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

    public Vector2Int GetHighProbabilityCoord(float per=0.5f)
    {
        if (!TryGetHighProbabilityCoord(per, out var target))
        {
            return Vector2Int.zero;
        }

        return target;
    }

    public bool TryGetHighProbabilityCoord(float per, out Vector2Int target)
    {
        // 按概率排序
        List<KeyValuePair<Vector2Int, int>> probability_map_list = probability_map.ToList().Where(kv=>kv.Value>0).OrderByDescending(kv => kv.Value).ToList();
        if (probability_map_list.Count == 0)
        {
            target = default;
            return false;
        }

        int takeCount = Mathf.Max(1, (int)(probability_map_list.Count * per));
        List<Vector2Int> max_map = probability_map_list.Take(takeCount).Select(kv=>kv.Key).ToList();

        // 随机选择一个最大概率点进行攻击
        target = max_map[SeedController.instance.Range(0, max_map.Count)];
        return true;
    }

    public int GetHighProbabilityRowWithout(List<int> without_row)
    {
        if (!TryGetHighProbabilityRowWithout(without_row, out var row))
        {
            return -1;
        }

        return row;
    }

    public bool TryGetHighProbabilityRowWithout(List<int> without_row, out int row)
    {
        // 按概率排序
        List<KeyValuePair<int, int>> probability_map_list = probability_map.ToList().Where(kv => kv.Value > 0).GroupBy(kv => kv.Key.x).Select(g => new KeyValuePair<int,int>(g.Key,g.Sum(kv => kv.Value))).OrderByDescending(kv => kv.Value).ToList();
        foreach(var kv in probability_map_list)
        {
            if(!without_row.Contains(kv.Key))
            {
                row = kv.Key;
                return true;
            }
        }
        row = -1;
        return false;
    }

    public LayoutDATA GetHighProbabilityRange(Vector2Int target,LayoutDATA layout)
    {
        List<KeyValuePair<LayoutDATA, int>> probability_map_list = layout.AllLayout().ConvertAll(v=>new KeyValuePair<LayoutDATA, int>(v,probability_map.GetProbability(target,v.ToList)));
        // 按概率排序
        List<LayoutDATA> max_map = probability_map_list.Where(kv => kv.Value >= probability_map_list[0].Value).Select(kv=>kv.Key).ToList();

        return max_map[0];
    }
    
    public string GetProbabilityMap()
    {
        return probability_map.ToString();
    }

    public bool CheckAvailable(Vector2Int coord)
    {
        return probability_map.Check(coord);
    }

    bool TryRemoveLayout(LayoutDATA target, out LayoutDATA removedLayout)
    {
        for (int i = 0; i < valid_layouts_inMap.Count; i++)
        {
            var layout = valid_layouts_inMap[i];
            if (!IsSameLayout(layout, target))
            {
                continue;
            }

            removedLayout = layout;
            valid_layouts_inMap.RemoveAt(i);
            return true;
        }

        removedLayout = null;
        return false;
    }

    bool IsSameLayout(LayoutDATA a, LayoutDATA b)
    {
        if (a == null || b == null || a.CoreNumber != b.CoreNumber)
        {
            return false;
        }

        if (a.ToList.Count != b.ToList.Count)
        {
            return false;
        }

        for (int i = 0; i < a.ToList.Count; i++)
        {
            if (a.ToList[i] != b.ToList[i])
            {
                return false;
            }
        }

        return true;
    }
}
