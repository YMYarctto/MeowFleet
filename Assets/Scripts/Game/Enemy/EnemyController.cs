using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject ship_coord__test;
    public GameObject hit_point__test;
    public bool isTest;

    public Vector2Int size;
    public List<Ships_Enum> target_ships_enum;

    Transform playArea;

    List<List<Vector2Int>> target_ship;

    List<Vector2Int> available_map;
    Dictionary<List<Vector2Int>, List<Vector2Int>> in_map_layout;

    EnemyBehavior AI;

    void Awake()
    {
        target_ship = target_ships_enum.ConvertAll(ship_enum =>
        {
            ShipData ship_data = DataManager.instance.GetShipData(ship_enum);
            return ship_data.shape_coord;
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
                for (int i = 0; i < 100; i++)
                {
                    System.Random rand = new();
                    Vector2Int pos = available_map[rand.Next(available_map.Count)];
                    if (CheckLayoutValid(pos, layout))
                    {
                        in_map_layout.Add(layout.ConvertAll(coord => pos + coord), layout);
                        foreach (var coord in layout)
                        {
                            available_map.Remove(pos + coord);
                            if(isTest)DrawMap__test(pos + coord, ship_coord__test);
                        }
                        break;
                    }
                    if (i == 99)
                    {
                        Debug.LogError("无法在地图中放置所有目标舰船");
                    }
                }
            }
        });
        task.Wait();
    }

    // 检查该点位该布局是否可行
    private bool CheckLayoutValid(Vector2Int center, List<Vector2Int> layout)
    {
        foreach (var coord in layout)
        {
            if (!available_map.Contains(center + coord))
                return false;
        }
        return true;
    }

    public void DrawMap__test(Vector2Int target_coord,GameObject perfab)
    {
        Vector3 coord_in_map = new Vector3(target_coord.x*100 + 50, target_coord.y*100 + 50);
        GameObject obj = Instantiate(perfab, coord_in_map, Quaternion.identity);
        obj.transform.SetParent(playArea);
    }

}
