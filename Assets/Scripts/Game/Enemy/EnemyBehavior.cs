using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

// 敌人行为算法
public class EnemyBehavior
{
    Status current_status;

    Queue<List<Vector2Int>> hit_ship_queue;
    List<Vector2Int> current_hit_ship;

    // 目标舰船布局
    List<List<Vector2Int>> target_ship;
    List<List<Vector2Int>> target_ship_spin
    {
        get=>target_ship.ConvertAll(layout=>
        {
            List<Vector2Int> new_layout = new();
            layout.ForEach(coord =>
            {
                new_layout.Add(new Vector2Int(coord.y, coord.x));
            });
            return new_layout;
        });
    }

    // 攻击地图
    List<Vector2Int> hit_map;

    // 概率密度图
    Dictionary<Vector2Int, int> possible_map_hunt;
    Dictionary<Vector2Int, int> possible_map_target;

    // 有效布局
    List<List<Vector2Int>> valid_layouts_hunt;
    List<List<Vector2Int>> valid_layouts_target;

    // 初始化
    public void Init(Vector2Int size, List<List<Vector2Int>> target_ships)
    {
        current_status = Status.Hunt;
        hit_map = new();
        possible_map_hunt = new();
        possible_map_target = new();
        valid_layouts_hunt = new();
        valid_layouts_target = new();
        Task task = Task.Run(() =>
        {
            // 初始化目标舰船布局
            target_ship = new(target_ships);

            // 初始化攻击地图
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    hit_map.Add(new Vector2Int(x, y));
                }
            }

            // 初始化概率密度图和有效布局
            List<List<Vector2Int>> target_ship_total = new(target_ships);
            target_ship_total.AddRange(target_ship_spin);
            foreach (var target in target_ship_total)
            {
                foreach (Vector2Int coord in hit_map)
                {
                    if (CheckLayoutValid(coord, target))
                    {
                        valid_layouts_hunt.Add(target);
                        foreach (var v2 in target)
                        {
                            Vector2Int pos = coord + v2;
                            if (possible_map_hunt.ContainsKey(pos))
                            {
                                possible_map_hunt[pos]++;
                            }
                            else
                            {
                                possible_map_hunt[pos] = 1;
                            }
                        }
                    }
                }
            }
        });
        task.Wait();
    }
    
    public Vector2Int CalculatePossibleMap()
    {
        // 根据当前状态选择概率密度图
        Dictionary<Vector2Int, int> possible_map = current_status == Status.Hunt ? possible_map_hunt : possible_map_target;

        Task<Vector2Int> task = Task.Run(() =>
        {
            // 按概率排序
            List<KeyValuePair<Vector2Int, int>> possible_map_list = possible_map.ToList();
            possible_map_list.OrderByDescending(kv => kv.Value);

            // 随机选择一个较大概率点进行攻击
            System.Random rand = new();
            Vector2Int target = possible_map_list[rand.Next(possible_map_list.Count / 4)].Key;
            return target;
        });
        
        return task.Result;
    }
    
    public void UpdatePossibleMapAfterAction(Vector2Int target, HitResult result,List<List<Vector2Int>> hit_ship=null)
    {
        Task task = Task.Run(() =>
        {
            // 删除该点位的概率布局和有效布局
            foreach (var layout in valid_layouts_hunt)
            {
                if (layout.Contains(target))
                {
                    DeletePossiblePoint(possible_map_hunt,valid_layouts_hunt,target, layout);
                }
            }
        });

        task.Wait();

        hit_map.Remove(target);
        if (result == HitResult.Hit)
        {
            if (current_status == Status.Hunt)
            {
                // 记录首次击中
                hit_ship.ForEach(layout => hit_ship_queue.Enqueue(layout));
                current_status = Status.Target;
                current_hit_ship ??= hit_ship_queue.Dequeue();

                // 初始化目标模式的概率密度图和有效布局
                possible_map_target = new();
                valid_layouts_target = new();
                task = Task.Run(() =>
                {
                    foreach (var coord in hit_map)
                    {
                        if (CheckLayoutPassBy(coord, current_hit_ship, target))
                        {
                            AddPossiblePointOnTarget(coord, current_hit_ship);//bug
                        }
                    }
                });

                task.Wait();
            }
            

        }else if (result == HitResult.Destroyed)
        {
            List<Vector2Int> destroy_layout = hit_ship_queue.Dequeue();
            List<Vector2Int> destroy_layout_spin = destroy_layout.ConvertAll(coord => new Vector2Int(coord.y, coord.x));
            target_ship.Remove(destroy_layout);
            if (hit_ship_queue.Count == 0)
            {
                current_status = Status.Hunt;
                current_hit_ship = null;
            }

            // 删除击沉舰船的所有有效布局
            task = Task.Run(() =>
            {
                foreach (var v2 in hit_map)
                {
                    DeletePossiblePoint(possible_map_hunt,valid_layouts_hunt,v2, destroy_layout);
                    DeletePossiblePoint(possible_map_hunt,valid_layouts_hunt,v2, destroy_layout_spin);
                }
            });
            
            task.Wait();
        }
    }

    // 检查该点位该布局是否可行
    private bool CheckLayoutValid(Vector2Int center, List<Vector2Int> layout)
    {
        foreach (var coord in layout)
        {
            if (!hit_map.Contains(center + coord))
                return false;
        }
        return true;
    }

    private bool CheckLayoutPassBy(Vector2Int center, List<Vector2Int> layout,Vector2Int target)
    {
        foreach (var coord in layout)
        {
            if (coord + center == target)
                return true;
        }
        return false;
    }

    // 删除该点位该布局的概率
    private void DeletePossiblePoint(Dictionary<Vector2Int, int> current_map,List<List<Vector2Int>> current_layouts,Vector2Int center, List<Vector2Int> layout)
    {
        if (!CheckLayoutValid(center, layout))
        {
            return;
        }
        current_layouts.Remove(layout);
        foreach (var coord in layout)
        {
            current_map[center + coord]--;
        }
    }
    
    private void AddPossiblePointOnTarget(Vector2Int center, List<Vector2Int> layout)
    {
        if (!CheckLayoutValid(center, layout))
        {
            return;
        }
        valid_layouts_target.Add(layout);
        foreach (var coord in layout)
        {
            if (!possible_map_target.ContainsKey(center + coord))
            {
                possible_map_target[center + coord] = 0;
            }
            possible_map_target[center + coord]++;
        }
    }

    enum Status
    {
        Hunt,
        Target,
    }

    public enum HitResult
    {
        Miss,
        Hit,
        Destroyed,
    }
}
