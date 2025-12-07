using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PVEController : MonoBehaviour
{
    readonly string CORE = "核心";
    readonly string BODY = "船体";
    readonly string DESTROY = "击沉";
    readonly string CAPTURE = "俘获";

    Dictionary<Vector2Int,int> player_ships_id;
    Dictionary<Vector2Int,Ship> player_ships;
    LayoutMap player_layout_map;
    GridCellGroup_Player gridCellGroup_Player;
    GridCellGroup_Enemy gridCellGroup_Enemy;
    List<Vector2Int> current_range;
    PVEMap pve_map=PVEMap.Null;

    Transform ShipGroupTrans;
    StageNotice_Animator stage_notice;
    
    SkillArea skillArea;
    Skill current_skill;

    PVEState currentState;

    bool PlayerAction => currentState == PVEState.PlayerAttack||currentState==PVEState.PlayerSkill;

    public int Round;
    public int PlayerShootCount => player_layout_map.AttackCount;
    int current_shoot_count;

    public Transform UI_Notice;

    float stage_animation_time = 0.8f;

    bool init=false;

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
        Global.PPU = 6.25f;
        DataManager.instance.SaveData.GetFormationData(out player_ships_id);

        UI_Notice = GameObject.Find("UI_Notice").transform;

        ShipGroupTrans = GameObject.Find("ShipGroup").transform;
        player_ships = player_ships_id.ToDictionary(kv => kv.Key, kv => ShipManager.instance.GetShip(kv.Value));

        player_layout_map = new();

        foreach (var kv in player_ships_id)
        {
            Ship ship = player_ships[kv.Key];
            player_layout_map.AddShip(ship.Uid, kv.Key, ship.Layout);
        }

        currentState = PVEState.PlayerSkill;
        current_range = new List<Vector2Int>() { new(0, 0) };
        Round = 1;

        current_shoot_count = PlayerShootCount;
    }

    public void Init()
    {
        init = true;
        PlayerSkillTurn();
        stage_notice.Open_PlayerSkill();
    }

    void Start()
    {
        gridCellGroup_Player = UIManager.instance.GetUIView<GridCellGroup_Player>();
        gridCellGroup_Enemy = UIManager.instance.GetUIView<GridCellGroup_Enemy>();
        stage_notice = UIManager.instance.GetUIView<StageNotice_Animator>();
        skillArea = UIManager.instance.GetUIView<SkillArea>();
        foreach (var kv in player_ships_id)
        {
            Ship ship = player_ships[kv.Key];
            skillArea.AddSkillCard(kv.Value,ship);
            GameObject obj = Ship_UIBase.Create<Ship_PVE>(kv.Value, ship, ShipGroupTrans);
            StartCoroutine(SetPosition_WaitForEndOfFrame(kv.Key, obj.GetComponent<Ship_PVE>()));
        }
    }

    IEnumerator SetPosition_WaitForEndOfFrame(Vector2Int coord, Ship_PVE ship)
    {
        yield return new WaitForEndOfFrame();
        ship.SetPosition(coord);
    }

    public void PlayerSkillTurn()
    {
        pve_map = PVEMap.Null;
    }

    public void PlayerAttackTurn()
    {
        current_shoot_count = PlayerShootCount;
        pve_map = PVEMap.Enemy;
        current_range = new List<Vector2Int>() { new(0, 0) };
    }

    public void ResetPVEMap()
    {
        pve_map = PVEMap.Null;
    }

    public ActionMessage EnemyAttack(Vector2Int coord)
    {
        ActionMessage message = player_layout_map.GetMessage(coord);
        gridCellGroup_Player.Hit(coord,!message.Contains(ActionMessage.ActionResult.Miss));
        return message;
    }

    public void PlayerSelect(Vector2Int coord,PVEMap target)
    {
        if (!PlayerAction||target!=pve_map||!init)
        {
            return;
        }
        if(target==PVEMap.Enemy)
        {
            gridCellGroup_Enemy.Select(current_range.ConvertAll(v => v + coord).ToList());
        }else if(target==PVEMap.Player)
        {
            gridCellGroup_Player.Select(current_range.ConvertAll(v => v + coord).ToList());
        }
    }

    public void PlayerOP(Vector2Int coord,PVEMap target)
    {
        if (!PlayerAction||target!=pve_map||!init)
        {
            return;
        }
        if(currentState==PVEState.PlayerAttack)
        {
            PlayerHit(coord);
        }
        else if(currentState==PVEState.PlayerSkill)
        {
            current_skill?.OnSkill(coord);
        }
    }

    public void PlayerHit(Vector2Int coord)
    {
        List<Vector2Int> coords = current_range.ConvertAll(v => v + coord).ToList();
        foreach (var v2 in new List<Vector2Int>(coords))
        {
            ActionMessage message = EnemyController.instance.PlayerHit(v2);
            Debug.Log(message);
            gridCellGroup_Enemy.Hit(v2,!message.Contains(ActionMessage.ActionResult.Miss));
            if (message.Contains(ActionMessage.ActionResult.Hit))
            {
                string LOCATE = message.Locate == ActionMessage.ActionLocate.core ? CORE : BODY;
                PVE_Notice notice = PVE_Notice.Create();
                notice.ShowNotice_Hit(message.ShipName, LOCATE);
            }
            else if (message.Contains(ActionMessage.ActionResult.Destroyed))
            {
                string ACTION = message.Locate == ActionMessage.ActionLocate.core ? DESTROY : CAPTURE;
                PVE_Notice notice = PVE_Notice.Create();
                notice.ShowNotice_Destroy(message.ShipName, ACTION);
            }
            if(message.Contains(ActionMessage.ActionResult.GameOver))
            {
                Debug.Log("Win");

                //TODO
            }
        }

        if(currentState==PVEState.PlayerAttack)
        {
            current_shoot_count--;
            Debug.Log($"剩余次数: {current_shoot_count}");
            if(current_shoot_count<=0)
            {
                pve_map = PVEMap.Null;
            }
        }
    }
    
    public void SetSkill(Skill skill)
    {
        current_skill = skill;
        current_range = skill.SkillRange;
        pve_map = skill.TargetMap;
    }

    public void ClearSelectedSkill()
    {
        current_skill = null;
        pve_map = PVEMap.Null;
    }

    public void ClearSelect()
    {
        gridCellGroup_Player.ClearSelect();
        gridCellGroup_Enemy.ClearSelect();
    }

    public void NextState()
    {
        if(currentState==PVEState.PlayerSkill||currentState==PVEState.PlayerAttack)
        {
            NextState(1);
        }
    }

    public void NextRound()
    {
        Round++;
        UIManager.instance.GetUIView<RoundNotice_Animator>().ChangeRound(Round);
        NextState(1);
    }

    public void NextState(int n)
    {
        int next = ((int)currentState + n) % System.Enum.GetValues(typeof(PVEState)).Length;
        currentState = (PVEState)next;
        Debug.Log(currentState.ToString());
        if (currentState == PVEState.EnemyAttack)
        {
            EventManager.instance.Invoke(EventRegistry.PVE.EnemyTurn);
            stage_notice.Close();
        }
        if(currentState==PVEState.PlayerAttack)
        {
            skillArea.ShowSkill(false);
            PlayerAttackTurn();
            stage_notice.Close_Open_PlayerAttack(stage_animation_time);
        }
        if(currentState==PVEState.PlayerSkill)
        {
            skillArea.ShowSkill();
            PlayerSkillTurn();
            stage_notice.Open_PlayerSkill();
        }
    }

    public List<int> GetPlayerShipsID()
    {
        return player_ships.Values.Select(ship => ship.Uid).ToList();
    }

    public enum PVEState
    {
        PlayerSkill,
        PlayerAttack,
        //EnemySkill,
        EnemyAttack
    }

    public enum PVEMap
    {
        Player,
        Enemy,
        Null,
    }
}
