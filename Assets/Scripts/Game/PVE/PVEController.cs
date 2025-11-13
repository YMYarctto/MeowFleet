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
    BG_PVE bg;

    PVEState currentState;

    bool PlayerAction => currentState == PVEState.PlayerAttack;

    public int Round;
    public int PlayerShootCount => player_layout_map.Count;
    int current_shoot_count;

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
        Round = 1;

        current_shoot_count = PlayerShootCount;
    }

    void Start()
    {
        gridCellGroup_Player = UIManager.instance.GetUIView<GridCellGroup_Player>();
        gridCellGroup_Enemy = UIManager.instance.GetUIView<GridCellGroup_Enemy>();
        bg = UIManager.instance.GetUIView<BG_PVE>();
        bg.SetInteractionActive(false);
    }

    IEnumerator SetPosition_WaitForEndOfFrame(Vector2Int coord, Ship_PVE ship)
    {
        yield return new WaitForEndOfFrame();
        ship.SetPosition(coord);
    }

    public void PlayerTurn()
    {
        bg.SetInteractionActive(false);
        current_shoot_count = PlayerShootCount;
    }

    public ActionMessage EnemyAttack(Vector2Int coord)
    {
        gridCellGroup_Player.Hit(coord);
        return player_layout_map.GetMessage(coord);
    }

    public void PlayerSelect(Vector2Int coord)
    {
        if (!PlayerAction)
        {
            return;
        }
        gridCellGroup_Enemy.Select(current_range.ConvertAll(v => v + coord).ToList());
    }

    public void PlayerHit(Vector2Int coord)
    {
        List<Vector2Int> coords = current_range.ConvertAll(v => v + coord).ToList();
        gridCellGroup_Enemy.Hit(coords);
        foreach (var v2 in new List<Vector2Int>(coords))
        {
            ActionMessage message = EnemyController.instance.PlayerHit(v2);
            Debug.Log(message);
        }

        current_shoot_count--;
        Debug.Log($"剩余次数: {current_shoot_count}");
        if(current_shoot_count<=0)
        {
            NextState();
        }
    }

    public void ClearSelect()
    {
        gridCellGroup_Enemy.ClearSelect();
    }

    public void NextState()
    {
        int next = ((int)currentState + 1) % System.Enum.GetValues(typeof(PVEState)).Length;
        currentState = (PVEState)next;
        Debug.Log(currentState.ToString());
        if (currentState == PVEState.EnemyAttack)
        {
            EventManager.instance.Invoke(EventRegistry.PVE.EnemyTurn);
            bg.SetInteractionActive(true);
        }
        if(currentState==PVEState.PlayerAttack)
        {
            PlayerTurn();
        }
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
