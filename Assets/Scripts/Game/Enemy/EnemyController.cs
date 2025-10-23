using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject ship_coord__test;
    public GameObject hit_point__test;
    public bool isTest;

    public Vector2Int size;
    public List<Ships_Enum> target_ships_enum;

    Transform playArea;

    List<LayoutDATA> target_ship;

    List<Vector2Int> available_map;
    Dictionary<Vector2Int, LayoutDATA> in_map_layout;

    EnemyBehavior AI;

    void Awake()
    {
        target_ship = target_ships_enum.ConvertAll(ship_enum =>
        {
            ShipData ship_data = DataManager.instance.GetShipData(ship_enum);
            return new LayoutDATA(ship_data.shape_coord);
        });

        if (isTest)
        {
            playArea = GameObject.Find("PlayArea").transform;
        }

        MapInit();

        AI = new EnemyBehavior();
        AI.Init(size, target_ship);
    }

    private void MapInit()
    {
        available_map = new();
        in_map_layout = new();

        // 随机放置舰船
        Task task = Task.Run(() =>
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    available_map.Add(new Vector2Int(x, y));
                }
            }

            foreach (var layout in target_ship)
            {
                for (int i = 0; i < 1000; i++)
                {
                    System.Random rand = new();
                    LayoutDATA layout_ran = rand.Next(2)==0?layout:new(layout.Current.ConvertAll(coord=>new Vector2Int(coord.y,coord.x)));
                    Vector2Int pos = available_map[rand.Next(available_map.Count)];
                    if (CheckLayoutValid(pos, layout_ran))
                    {
                        in_map_layout.Add(pos, layout_ran);//?
                        foreach (var coord in layout_ran.Current)
                        {
                            available_map.Remove(pos + coord);
                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                if(isTest)DrawMap__test(pos + coord, ship_coord__test);
                            });
                        }
                        break;
                    }
                    if (i == 999)
                    {
                        Debug.LogError("无法在地图中放置所有目标舰船");
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
            if (!available_map.Contains(center + coord))
                return false;
        }
        return true;
    }

    public void DrawMap__test(Vector2Int target_coord,GameObject perfab)
    {
        Vector3 coord_in_map = new Vector3(target_coord.x*100 + 50, target_coord.y*100 + 50);
        GameObject obj = Instantiate(perfab);
        obj.transform.SetParent(playArea);
        obj.transform.localPosition = coord_in_map;
    }

}
