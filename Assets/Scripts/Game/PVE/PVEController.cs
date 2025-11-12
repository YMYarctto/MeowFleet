using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PVEController : MonoBehaviour
{
    Dictionary<Vector2Int,int> player_ships_id;
    Dictionary<Vector2Int,Ship> player_ships;
    LayoutMap player_layout_map;
    GridCellGroup_Player gridCellGroup_Player;
    GridCellGroup_Enemy gridCellGroup_Enemy;
    List<Vector2Int> current_range;

    Transform ShipGroupTrans;
    
    PVEState currentState;

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
        DataManager.instance.SaveData.GetFormationData(out player_ships_id);

        ShipGroupTrans = GameObject.Find("ShipGroup").transform;
        player_ships = player_ships_id.ToDictionary(kv => kv.Key, kv => ShipManager.instance.GetShip(kv.Value));

        player_layout_map = new();

        foreach (var kv in player_ships_id)
        {
            player_layout_map.AddShip(kv.Value, kv.Key, player_ships[kv.Key].Layout);
            GameObject obj = Ship_UIBase.Create<Ship_PVE>(kv.Value, player_ships[kv.Key], ShipGroupTrans);
            StartCoroutine(SetPosition_WaitForEndOfFrame(kv.Key, obj.GetComponent<Ship_PVE>()));
        }

        currentState = PVEState.PlayerAttack;
        current_range = new List<Vector2Int>() { new(0, 0) };
    }

    void Start()
    {
        gridCellGroup_Player = UIManager.instance.GetUIView<GridCellGroup_Player>();
        gridCellGroup_Enemy = UIManager.instance.GetUIView<GridCellGroup_Enemy>();
    }

    IEnumerator SetPosition_WaitForEndOfFrame(Vector2Int coord, Ship_PVE ship)
    {
        yield return new WaitForEndOfFrame();
        ship.SetPosition(coord);
    }

    public ActionMessage GetPlayerMessage(Vector2Int coord)
    {
        gridCellGroup_Player.Hit(coord);
        return player_layout_map.GetMessage(coord);
    }

    public void PlayerSelect(Vector2Int coord)
    {
        if (currentState != PVEState.PlayerAttack)
        {
            return;
        }
        gridCellGroup_Enemy.Select(current_range.ConvertAll(v => v + coord).ToList());
    }

    public void ClearSelect()
    {
        gridCellGroup_Enemy.ClearSelect();
    }

    public void NextState()
    {
        int next = ((int)currentState + 1) % System.Enum.GetValues(typeof(PVEState)).Length;
        currentState = (PVEState)next;
    }

    public List<int> GetPlayerShipsID()
    {
        return player_ships.Values.Select(ship => ship.Uid).ToList();
    }

    public enum PVEState
    {
        //PlayerSkill,
        PlayerAttack,
        //EnemySkill,
        EnemyAttack
    }
}
