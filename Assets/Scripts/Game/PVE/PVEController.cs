using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PVEController : MonoBehaviour
{
    readonly string CORE = "核心";
    readonly string BODY = "船体";
    readonly string DESTROY = "击沉";
    readonly string CAPTURE = "俘获";

    Dictionary<Vector2Int,int> player_ships_id;
    Dictionary<Vector2Int,Ship> player_ships;
    List<Vector2Int> hitted_coords;
    LayoutMap player_layout_map;
    GridCellGroup_Player gridCellGroup_Player;
    GridCellGroup_Enemy gridCellGroup_Enemy;
    List<Vector2Int> current_skill_range;
    Vector2Int select = default;
    PVEMap pve_map=PVEMap.Null;
    Aim aim;

    public LayoutMap PlayerLayoutMap => player_layout_map;

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
        hitted_coords = new();

        foreach (var kv in player_ships_id)
        {
            Ship ship = player_ships[kv.Key];
            player_layout_map.AddShip(ship.Uid, kv.Key, ship.Layout);
        }

        currentState = PVEState.PlayerSkill;
        current_skill_range = new List<Vector2Int>() { new(0, 0) };
        Round = 1;

        current_shoot_count = PlayerShootCount;
    }

    public void Init()
    {
        init = true;
        PlayerSkillTurn();
        stage_notice.Open_PlayerSkill();
        InputController.instance.SelectActionMap(ActionMapRegistry.PVEMap);
    }

    void Start()
    {
        gridCellGroup_Player = UIManager.instance.GetUIView<GridCellGroup_Player>();
        gridCellGroup_Enemy = UIManager.instance.GetUIView<GridCellGroup_Enemy>();
        stage_notice = UIManager.instance.GetUIView<StageNotice_Animator>();
        skillArea = UIManager.instance.GetUIView<SkillArea>();
        aim = UIManager.instance.GetUIView<Aim>();
        foreach (var kv in player_ships_id)
        {
            Ship ship = player_ships[kv.Key];
            skillArea.AddSkillCard(kv.Value,ship);
            GameObject obj = Ship_UIBase.Create<Ship_PVE>(kv.Value, ship, ShipGroupTrans);
            StartCoroutine(SetPosition_WaitForEndOfFrame(kv.Key, obj.GetComponent<Ship_PVE>()));
        }
    }

    void OnEnable()
    {
        InputController.InputAction.PVEMap.Rotate.started += Rotate;
    }

    void OnDisable()
    {
        InputController.InputAction.PVEMap.Rotate.started -= Rotate;
    }

    void OnDestroy()
    {
        InputController.instance?.SelectActionMap(ActionMapRegistry.DefaultMap);
    }

    IEnumerator SetPosition_WaitForEndOfFrame(Vector2Int coord, Ship_PVE ship)
    {
        yield return new WaitForEndOfFrame();
        ship.SetPosition(coord);
    }

    // Pack

    public void PlayerSkillTurn()
    {
        pve_map = PVEMap.Null;
    }

    public void PlayerAttackTurn()
    {
        current_shoot_count = PlayerShootCount;
        pve_map = PVEMap.Enemy;
        current_skill_range = new List<Vector2Int>() { new(0, 0) };
    }

    public void ResetPVEMap()
    {
        pve_map = PVEMap.Null;
    }

    public void DisposeMessage(ActionMessage message)
    {
        player_layout_map.DisposeMessage(message);
    }

    // Behavior

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
        select = coord;
        if(current_skill is radar)
        {
            coord = GetEdgeCoord(coord, current_skill.Direction);
            gridCellGroup_Enemy.Select(current_skill_range.ConvertAll(v => v + coord)
                .Union(EnemyController.instance.GetWholeLine(coord, current_skill.Direction)).ToList());
            return;
        }
        if(target==PVEMap.Enemy)
        {
            gridCellGroup_Enemy.Select(current_skill_range.ConvertAll(v => v + coord).ToList());
        }else if(target==PVEMap.Player)
        {
            gridCellGroup_Player.Select(current_skill_range.ConvertAll(v => v + coord).ToList());
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

    public bool PlayerHit(Vector2Int coord)
    {
        List<Vector2Int> coords = current_skill_range.ConvertAll(v => v + coord).ToList();
        List<ActionMessage> messages = new();
        // UI以及合并逻辑初始化
        foreach (var v2 in new List<Vector2Int>(coords))
        {
            if(hitted_coords.Contains(v2))continue;
            hitted_coords.Add(v2);
            ActionMessage message = EnemyController.instance.PlayerHit(v2);
            Debug.Log(message);
            messages.Add(message);
            gridCellGroup_Enemy.Hit(v2,!message.Contains(ActionMessage.ActionResult.Miss));
        }
        EnemyController.instance.DisposeMessage(messages);
        // 判断是否结束
        if(messages.Any(v=>v.Contains(ActionMessage.ActionResult.GameOver)))
        {
            Debug.Log("Win");

            //TODO
        }
        // 分类合并
        messages = messages.Where(v=>!v.Contains(ActionMessage.ActionResult.Miss))
            .GroupBy(v=>(v.ShipID,v.Contains(ActionMessage.ActionResult.Hit)&&v.Locate==ActionMessage.ActionLocate.core,v.Contains(ActionMessage.ActionResult.Hit)&&v.Locate==ActionMessage.ActionLocate.body))
            .Select(v=>v.FirstOrDefault(v=>v.Contains(ActionMessage.ActionResult.Destroyed)&&v.Locate==ActionMessage.ActionLocate.core)??v.First()).ToList();
        // 处理结果
        foreach (var message in messages)
        {
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
        // 减少齐射次数
        if(currentState==PVEState.PlayerAttack)
        {
            current_shoot_count--;
            Debug.Log($"剩余次数: {current_shoot_count}");
            if(current_shoot_count<=0)
            {
                pve_map = PVEMap.Null;
            }
        }

        return messages.Count>0 && messages.Any(v=>!v.Contains(ActionMessage.ActionResult.Miss));
    }
    
    public void SetSkill(Skill skill)
    {
        current_skill = skill;
        current_skill_range = skill.SkillRange;
        pve_map = skill.TargetMap;
    }

    // Clear

    public void ClearSelectedSkill()
    {
        current_skill = null;
        pve_map = PVEMap.Null;
    }

    public void ClearSelect()
    {
        gridCellGroup_Player.ClearSelect();
        gridCellGroup_Enemy.ClearSelect();
        select = default;
    }

    // Next

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

    // Get

    public List<int> GetPlayerShipsID()
    {
        return player_ships.Values.Select(ship => ship.Uid).ToList();
    }

    public Vector2Int GetEdgeCoord(Vector2Int target,Vector2Int direction)
    {
        Vector2Int mapSize = EnemyController.instance.size;
        Vector2Int edge = target;
        while (true)
        {
            Vector2Int next = edge - direction;
            if (next.x < 0 || next.y < 0 || next.x >= mapSize.x || next.y >= mapSize.y)
            {
                break;
            }
            edge = next;
        }
        return edge;
    }

    // UI

    public void Aim(PVEMap target,Vector2 position)
    {
        if(!init)return;
        if(!PlayerAction||target!=pve_map)
        {
            aim.Disable();
            return;
        }
        aim.MoveTo(position);
        aim.Enable();
    }

    public void Aim(bool enable)
    {
        if(!init)return;
        if(!enable)
        {
            aim.Disable();
        }
    }

    // Input

    public void Rotate(InputAction.CallbackContext ctx)
    {
        int direction = (int)ctx.ReadValue<float>();
        LayoutDATA skill_rotated_layout = new(current_skill_range,0);
        current_skill_range = skill_rotated_layout.Rotate(direction).ToList;
        current_skill?.Rotate(direction);
        Vector2Int current_select = select;
        ClearSelect();
        if(current_select!=default&&pve_map!=PVEMap.Null)
        {
            PlayerSelect(current_select,pve_map);
        }
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
