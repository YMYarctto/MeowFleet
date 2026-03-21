using System.Collections.Generic;
using UnityEngine;

// 敌人行为算法
public class EnemyBehavior
{
    int current_target_index=-1;

    // 攻击地图
    List<Vector2Int> hit_map;

    // 概率密度图
    // -1 => hunt
    Dictionary<int, ProbabilityMap> map_dict;
    ProbabilityMap hunt_map => map_dict[-1];

    // 初始化
    public void Init(Vector2Int size, List<LayoutDATA> target_ship)
    {
        hit_map = new();
        map_dict = new();
        map_dict[-1] = new ProbabilityMap();
        // 初始化攻击地图
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                hit_map.Add(new Vector2Int(x, y));
            }
        }

        // 初始化概率密度图和有效布局
        List<LayoutDATA> target_ship_total = new();
        foreach(var target in target_ship)
        {
            foreach(var layout in target.AllLayout())
            {
                target_ship_total.Add(layout);
                foreach (Vector2Int coord in hit_map)
                {
                    if (CheckLayoutValid(coord, layout))
                    {
                        hunt_map.AddProbability(coord, layout);
                    }
                }
            }
        }
    }

    public Vector2Int CalculateHuntMap(float per=0.5f)
    {
        if (hunt_map.TryGetHighProbabilityCoord(per, out var target))
        {
            return target;
        }

        return GetRandomTarget();
    }
    
    public Vector2Int CalculatePossibleMap(float per=0.5f)
    {
        // 选择概率密度图
        ProbabilityMap map = GetCurrentMap();
        if (map != null && map.TryGetHighProbabilityCoord(per, out var target))
        {
            return target;
        }

        return GetRandomTarget();
    }

    public int CalculatePossibleMapWithoutRow(List<int> without_row)
    {
        // 选择概率密度图
        ProbabilityMap map = GetCurrentMap();
        if (map != null && map.TryGetHighProbabilityRowWithout(without_row, out var row))
        {
            return row;
        }

        return GetRandomAvailableRow(without_row);
    }

    public List<Vector2Int> CalculateSkillRange(Vector2Int target,LayoutDATA range)
    {
        ProbabilityMap map = GetCurrentMap();
        return map.GetHighProbabilityRange(target,range).ToList;
    }

    public void UpdatePossibleMapAfterHit(Vector2Int target, KeyValuePair<int, LayoutDATA> hit_ship)
    {
        var layout = hit_ship.Value;
        if(hit_ship.Key>=0)
        {
            current_target_index = hit_ship.Key;
            if (map_dict.TryAdd(current_target_index, new ProbabilityMap(layout)))
            {
                // 新建概率布局
                current_target_index = hit_ship.Key;
                foreach (var coord in hit_map)
                {
                    foreach(var _layout in layout.AllLayout())
                    if (CheckLayoutPassBy(coord, _layout, target) && CheckLayoutValid(coord, _layout))
                    {
                        map_dict[current_target_index].AddProbability(coord, _layout);
                    }
                }
            }
        }
        foreach (var map_kv in map_dict)
        {
            if(map_kv.Value == null)
            {
                continue;
            }
            if (map_kv.Key != hit_ship.Key || map_kv.Key < 0)
            {
                // 未击中部分
                // 删除该点位的概率布局
                map_dict[map_kv.Key].DeleteProbability(target);
            }
            else
            {
                // 击中部分
                map_dict[current_target_index].DeleteProbabilityWithout(target);
            }
            map_dict[map_kv.Key].RemoveProbability(target);
        }
    }

    public void UpdatePossibleMapAfterDestroy(Vector2Int target, KeyValuePair<int, LayoutDATA> destroy_ship, LayoutDATA absolute_layout)
    {
        if (destroy_ship.Key>=0)
        {
            var kv = destroy_ship;
            var layout = kv.Value;

            foreach (var possibleLayout in layout.AllLayout())
            {
                foreach (var center in hit_map)
                {
                    if (CheckLayoutValid(center, possibleLayout))
                    {
                        hunt_map.DeleteProbabilityEach(center, possibleLayout);
                    }
                }
            }

            List<Vector2Int> invalidCoords = new();
            if (absolute_layout != null)
            {
                invalidCoords.AddRange(absolute_layout.ToList);
                foreach (var coord in absolute_layout.GetAdjacentCellsInMap(Vector2Int.zero))
                {
                    if (!invalidCoords.Contains(coord))
                    {
                        invalidCoords.Add(coord);
                    }
                }
            }

            foreach (var coord in invalidCoords)
            {
                hit_map.Remove(coord);
            }

            if (map_dict.TryGetValue(kv.Key, out var map) && map != null)
            {
                map_dict[kv.Key] = null;
            }

            foreach(var _map in map_dict.Values)
            {
                if (_map == null)
                {
                    continue;
                }
                foreach(var coord in invalidCoords)
                {
                    _map.DeleteProbability(coord);
                    _map.RemoveProbability(coord);
                }
            }
            // 更新目标
            current_target_index = -1;
            foreach (var target_kv in map_dict)
            {
                if (target_kv.Key>=0 && target_kv.Value != null)
                {
                    current_target_index = target_kv.Key;
                    break;
                }
            }
            Debug.Log("New Target Index: " + current_target_index);
        }
    }

    public void Remove(Vector2Int target)
    {
        hit_map.Remove(target);
    }
    
    public string GetCurrentProbabilityMap()
    {
        return GetCurrentMap()?.GetProbabilityMap() ?? "ProbabilityMapDATA:[\n]\n";
    }

    ProbabilityMap GetCurrentMap()
    {
        if (map_dict.TryGetValue(current_target_index, out var map) && map != null)
        {
            return map;
        }

        return hunt_map;
    }

    Vector2Int GetRandomTarget()
    {
        if (hit_map.Count == 0)
        {
            Debug.LogWarning("EnemyBehavior: no available target in hit_map");
            return Vector2Int.zero;
        }

        return hit_map[SeedController.instance.Range(0, hit_map.Count)];
    }

    int GetRandomAvailableRow(List<int> without_row)
    {
        List<int> rows = new();
        foreach (var coord in hit_map)
        {
            if (without_row.Contains(coord.x) || rows.Contains(coord.x))
            {
                continue;
            }

            rows.Add(coord.x);
        }

        if (rows.Count == 0)
        {
            Debug.LogWarning("EnemyBehavior: no available row outside excluded rows");
            return 0;
        }

        return rows[SeedController.instance.Range(0, rows.Count)];
    }

    // 检查该点位该布局是否可行
    private bool CheckLayoutValid(Vector2Int center, LayoutDATA layout)
    {
        foreach (var coord in layout.ToList)
        {
            if (!hit_map.Contains(center + coord))
                return false;
        }
        return true;
    }

    private bool CheckLayoutPassBy(Vector2Int center, LayoutDATA layout,Vector2Int target)
    {
        foreach (var coord in layout.ToList)
        {
            if (coord + center == target)
                return true;
        }
        return false;
    }
}
