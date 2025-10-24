using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Overlays;
using UnityEngine;

// 敌人行为算法
public class EnemyBehavior
{
    int current_target_index = -1;

    // 攻击地图
    List<Vector2Int> hit_map;

    // 概率密度图
    // -1 => hunt
    Dictionary<int, ProbabilityMap> map_dict;
    ProbabilityMap hunt_map{get => map_dict[-1];}

    // 初始化
    public void Init(Vector2Int size, List<LayoutDATA> target_ship)
    {
        hit_map = new();
        map_dict = new();
        map_dict[-1] = new ProbabilityMap();
        Task task = Task.Run(() =>
        {
            // 初始化攻击地图
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    hit_map.Add(new Vector2Int(x, y));
                }
            }

            // 初始化概率密度图和有效布局
            List<LayoutDATA> target_ship_total = new(target_ship);
            target_ship.ForEach(target=>target_ship_total.Add(target.Mirror()));
            foreach (var target in target_ship_total)
            {
                foreach (Vector2Int coord in hit_map)
                {
                    if (CheckLayoutValid(coord, target))
                    {
                        hunt_map.AddProbability(coord, target);
                    }
                }
            }
        });
        task.Wait();
    }
    
    public Vector2Int CalculatePossibleMap()
    {
        // 选择概率密度图
        ProbabilityMap map = map_dict[current_target_index];

        Task<Vector2Int> task = Task.Run(() =>
        {
            return map.GetHighProbabilityCoord();
        });
        
        return task.Result;
    }

    public void UpdatePossibleMapAfterHit(Vector2Int target, KeyValuePair<int, LayoutDATA> hit_ship)
    {
        Task task = Task.Run(() =>
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
                        if (CheckLayoutPassBy(coord, layout, target) && CheckLayoutValid(coord, layout))
                        {
                            map_dict[current_target_index].AddProbability(coord, layout);
                        }
                        if (CheckLayoutPassBy(coord, layout.Mirror(), target) && CheckLayoutValid(coord, layout.Mirror()))
                        {
                            map_dict[current_target_index].AddProbability(coord, layout.Mirror());
                        }
                    }
                }
            }
            foreach (var map_kv in map_dict)
            {
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
        });

        task.Wait();
    }

    public void UpdatePossibleMapAfterDestroy(KeyValuePair<int, LayoutDATA> destroy_ship)
    {
        Task task = Task.Run(() =>
        {
            if (destroy_ship.Key>=0)
            {
                var kv = destroy_ship;
                if (map_dict.TryGetValue(kv.Key, out var map) && map != null)
                {
                    map = null;

                    var layout = kv.Value;
                    foreach (var center in hit_map)
                    {
                        if (CheckLayoutValid(center, layout))
                        {
                            hunt_map.DeleteProbabilityEach(center, layout);
                        }
                    }

                    // 更新目标
                    if (!map_dict.All(kv =>
                    {
                        bool exist = kv.Key >= 0 && kv.Value != null;
                        if (exist)
                        {
                            current_target_index = kv.Key;
                        }
                        return exist;
                    }))
                    {
                        current_target_index = -1;
                    }
                }
            }
        });

        task.Wait();
    }

    public void Remove(Vector2Int target)
    {
        hit_map.Remove(target);
    }
    
    public string GetCurrentProbabilityMap()
    {
        return map_dict[current_target_index].GetProbabilityMap();
    }

    // 检查该点位该布局是否可行
    private bool CheckLayoutValid(Vector2Int center, LayoutDATA layout)
    {
        foreach (var coord in layout.Current)
        {
            if (!hit_map.Contains(center + coord))
                return false;
        }
        return true;
    }

    private bool CheckLayoutPassBy(Vector2Int center, LayoutDATA layout,Vector2Int target)
    {
        foreach (var coord in layout.Current)
        {
            if (coord + center == target)
                return true;
        }
        return false;
    }
}
