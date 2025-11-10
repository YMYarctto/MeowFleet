using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PVEController : MonoBehaviour
{
    Dictionary<Vector2Int,Ship> player_ships;
    LayoutMap player_layout_map;

    public Transform ShipGroupTrans{ get; private set; }

    private static PVEController _instance;
    public static PVEController instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(PVEController)) as PVEController;
                if (!_instance)
                {
                    Debug.LogError("场景中未找到 PVEController");
                    return null;
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        DataManager.instance.SaveData.GetFormationData(out var player_ships_id);

        ShipGroupTrans = GameObject.Find("ShipGroup").transform;
        player_ships = player_ships_id.ToDictionary(kv => kv.Key, kv => ShipManager.instance.GetShip(kv.Value));
    }

    public ActionMessage GetPlayerMessage(Vector2Int coord)
    {
        return player_layout_map.GetMessage(coord);
    }
}
