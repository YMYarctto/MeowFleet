using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Overlays;
using UnityEngine;

// 敌人行为算法
public class EnemyBehavior
{
    int current_target_index = -1;

    // 目标舰船布局
    List<LayoutDATA> target_ship;

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
            // 初始化目标舰船布局
            this.target_ship = new(target_ship);

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

    public void UpdatePossibleMapAfterHit(Vector2Int target, Dictionary<int, LayoutDATA> hit_ship)
    {
        Task task = Task.Run(() =>
        {
            foreach (var kv in hit_ship)
            {
                if (map_dict.TryGetValue(kv.Key, out var map) && map != null)
                {
                    // 删除该点位的概率布局
                    map.DeleteProbability(target, layout => CheckLayoutValid(target, layout));
                }
                else
                {
                    // 新建概率布局
                    var layout = kv.Value;
                    if (!map_dict.TryAdd(kv.Key, new ProbabilityMap(layout)))
                    {
                        continue;
                    }

                    foreach (var coord in hit_map)
                    {
                        if (CheckLayoutPassBy(coord, layout, target) && CheckLayoutValid(coord, layout))
                        {
                            map_dict[current_target_index].AddProbability(coord, layout);
                        }
                    }
                }
            }
        });

        task.Wait();
    }
    
    public void UpdatePossibleMapAfterDestroy(Dictionary<int, LayoutDATA> destroy_ship)
    {
        Task task = Task.Run(() =>
        {
            foreach (var kv in destroy_ship)
            {
                if(map_dict.TryGetValue(kv.Key, out var map) && map != null)
                {
                    map = null;

                    var layout = kv.Value;
                    foreach(var center in hit_map)
                    {
                        if(CheckLayoutValid(center,layout))
                        {
                            hunt_map.DeleteProbabilityEach(center, layout);
                        }
                    }
                }
            }
        });

        task.Wait();
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

    // 删除该点位该布局的概率
    private void DeletePossiblePoint(Dictionary<Vector2Int, int> current_map,List<LayoutDATA> current_layouts,Vector2Int center, LayoutDATA layout)
    {
        if (!CheckLayoutValid(center, layout))
        {
            return;
        }
        current_layouts.Remove(layout);
        foreach (var coord in layout.Current)
        {
            current_map[center + coord]--;
        }
    }

    // private void AddPossiblePointOnTarget(Vector2Int center, LayoutDATA layout)
    // {
    //     if (!CheckLayoutValid(center, layout))
    //     {
    //         return;
    //     }
    //     valid_layouts_target.Add(layout.Current);
    //     foreach (var coord in layout.Current)
    //     {
    //         if (!possible_map_target.ContainsKey(center + coord))
    //         {
    //             possible_map_target[center + coord] = 0;
    //         }
    //         possible_map_target[center + coord]++;
    //     }
    // }

    public enum HitResult
    {
        Miss,
        Hit,
        Destroyed,
    }
}
