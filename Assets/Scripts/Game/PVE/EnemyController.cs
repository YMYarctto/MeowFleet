using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    readonly string CORE = "核心";
    readonly string BODY = "船体";
    readonly string DESTROY = "击沉";
    readonly string CAPTURE = "俘获";
    readonly string SPYON = "侦查";

    public Vector2Int size => Vector2Int.one * LoadDataManager.instance.PVELoadData.EnemyMapSize;
    List<int> enemy_ships_id;

    List<int> target_ships_id;
    Queue<Vector2Int> target_list;

    EventGroup _event;
    GridCellGroup_Enemy gridCellGroup_Enemy;
    Sequence enemy_turn;

    List<LayoutDATA> target_ship;
    Dictionary<int,Ship> enemy_ship;
    List<Skill> enemy_skill;

    List<Vector2Int> available_map;
    LayoutMap layout_map;

    public LayoutMap EnemyLayoutMap => layout_map;

    EnemyBehavior AI;

    public int EnemyShootCount => layout_map.GetAttackCount();

    List<int> torpedo_hit;

    private static EnemyController _instance;
    public static EnemyController instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(EnemyController)) as EnemyController;
                if (!_instance)
                {
                    Debug.LogError("场景中未找到 EnemyController");
                    return null;
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        _event = EventManager.GroupBy("EnemyController");
        enemy_skill = new();
        gridCellGroup_Enemy = UIManager.instance.GetUIView<GridCellGroup_Enemy>();
        target_list = new();
        torpedo_hit = new();

        int ShipID = 20000;
        enemy_ships_id = LoadDataManager.instance.PVELoadData.EnemyGroupList;
        InformationBoard.GetUIView().InitInfoTitle(enemy_ships_id.Count);
        Dictionary<int,int> dataId_count = new();
        enemy_ship = enemy_ships_id.ConvertAll(id =>
        {
            ShipID++;
            Ship ship = new Ship(ShipID,DataManager.instance.GetShipData(id));
            Skill skill = Skill.Get(ship,null);
            if (skill != null)
            {
                enemy_skill.Add(skill);
            }
            if(dataId_count.TryGetValue(id,out int count))
            {
                dataId_count[id]++;
                ship.SetNameSuffix(++count);
            }
            else
            {
                dataId_count.Add(id,1);
                ship.SetNameSuffix(1);
            }
            return new KeyValuePair<int,Ship>(ShipID,ship);
        }).ToDictionary(kv=>kv.Key,kv=>kv.Value);

        enemy_skill.OrderBy(v => v.Order);

        GameObject info_window_perfab = ResourceManager.instance.GetPerfabByType<InformationWindow_UI>();
        Transform info_window_parent = GameObject.Find("InformationWindowGroup").transform;
        foreach(var kv in enemy_ship)
        {
            InformationWindow_UI.PrepareNextID(kv.Key);
            LayoutDATA layout = kv.Value.Layout;
            GameObject obj = Instantiate(info_window_perfab,info_window_parent);
            obj.GetComponent<InformationWindow_UI>().Init(layout.ToList,layout.CoreNumber,kv.Value.Name);
        }

        MapInit();

        AI = new EnemyBehavior();
        gridCellGroup_Enemy.LateInit();
    }

    void Start()
    {
        target_ships_id = PVEController.instance.GetPlayerShipsID();
        target_ship = target_ships_id.ConvertAll(id =>
        {
            ShipData ship_data = DataManager.instance.GetShipData(id);
            return new LayoutDATA(ship_data.shape_coord,ship_data.core_number);
        });

        AI.Init(PVEController.instance.size, target_ship);
    }

    void OnEnable()
    {
        _event.AddListener(EventRegistry.PVE.EnemyTurn, EnemyTurn);
    }

    void OnDisable()
    {
        _event?.RemoveListener(EventRegistry.PVE.EnemyTurn, EnemyTurn);
    }

    void OnDestroy()
    {
        enemy_turn?.Kill();
    }

    private void MapInit()
    {
        available_map = new();
        layout_map = new();

        // 随机放置舰船
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                available_map.Add(new Vector2Int(x, y));
            }
        }

        Tools.Shuffle(available_map,SeedController.instance.Random);
        for (int i=0,j=0;i<enemy_ship.Values.Count;i++)
        {
            Ship ship = enemy_ship.Values.ElementAt(i);
            LayoutDATA layout = new(ship.Layout);
            for (int e=0; j < available_map.Count; j++,e++)
            {
                List<LayoutDATA> layouts = layout.AllLayout();
                LayoutDATA layout_ran = layouts[SeedController.instance.Range(0, layouts.Count)];
                Vector2Int pos = available_map[j];
                if (i == enemy_ship.Values.Count - 1)
                {
                    available_map.Add(pos);
                }
                if (CheckLayoutValid(pos, layout_ran))
                {
                    ship.ForceSetLayout(layout_ran);
                    layout_map.AddShip(ship.ShipId,pos, ship);
                    foreach (var coord in layout_ran.ToList)
                    {
                        available_map.Remove(pos + coord);
                    }
                    foreach(var coord in layout_ran.GetAdjacentCellsInMap(pos))
                    {
                        available_map.Remove(coord);
                    }
                    break;
                }
                if (e > 1000)
                {
                    Debug.LogError("无法在地图中放置所有目标舰船");
                    break;
                }
            }
        }
    }

    public void EnemyTurn()
    {
        enemy_turn?.Kill();
        enemy_turn = DOTween.Sequence();
        for (int i = 0; i < enemy_skill.Count; i++)
        {
            if(!enemy_skill[i].CanSkill)continue;
            Skill skill = enemy_skill[i];
            enemy_turn.AppendInterval(0.5f);
            enemy_turn.AppendCallback(()=>ShipSkill(skill));
        }
        enemy_turn.AppendInterval(1f);
        for (int i = 0; i < EnemyShootCount; i++)
        {
            enemy_turn.AppendInterval(0.2f);
            enemy_turn.AppendCallback(()=>Attack());
        }
        enemy_turn.AppendInterval(1.5f);
        enemy_turn.AppendCallback(()=>PVEController.instance.NextRound());
        enemy_turn.Play();
    }

    public ActionMessage PlayerCheck(Vector2Int coord)
    {
        gridCellGroup_Enemy.Check(coord);
        return layout_map.Checkout(coord);
    }

    public ActionMessage PlayerHit(Vector2Int coord)
    {
        gridCellGroup_Enemy.Check(coord);
        return layout_map.GetMessage(coord);
    }

    public void DisposeMessage(List<ActionMessage> messages)
    {
        layout_map.DisposeMessage(messages);
    }
    
    private void Attack()
    {
        Vector2Int target_coord = GetTarget();
        FXManager.instance.AttackFX(PVEController.instance.GetPositionInScene(target_coord));
        ActionMessage message = PVEController.instance.EnemyAttack(target_coord);
        PVEController.instance.DisposeMessage(message);
        Debug.Log(message);
        if (message.Contains(ActionMessage.ActionResult.Destroyed))
        {
            AI.UpdatePossibleMapAfterDestroy(message.Target, message.DestroyedShips, message.DestroyedShipAbsoluteLayout);
        }
        else
        {
            AI.UpdatePossibleMapAfterHit(message.Target, message.HitShips);
        }
        MessageToInfo(message);

        AI.Remove(target_coord);
        Debug.Log(AI.GetCurrentProbabilityMap());

        if(message.Contains(ActionMessage.ActionResult.GameOver))
        {
            PVE_Notice.Create().ShowNotice_Defeat();
            Sequence sequence=DOTween.Sequence();
            sequence.AppendInterval(2f);
            sequence.AppendCallback(()=>SettlePage.GetUIView().Defeat());
            return;
        }
    }

    private void ShipSkill(Skill skill)
    {
        Vector2Int target_coord;
        List<ActionMessage> messages;
        if(skill is bomb)
        {
            goto Bomb;
        }
        else if(skill is torpedo)
        {
            goto Torpedo;
        }
        else if(skill is radar)
        {
            goto Radar;
        }
        else if(skill is interference)
        {
            goto Interference;
        }
        else
        {
            goto End;
        }
Bomb:
        target_coord = GetTarget(0.1f);
        List<Vector2Int> skill_range = AI.CalculateSkillRange(target_coord,new LayoutDATA(skill.SkillRange,0));
        
        messages = PVEController.instance.EnemyAttack(target_coord,skill_range);
        goto Update;
Torpedo:
        target_coord = new(AI.CalculatePossibleMapWithoutRow(torpedo_hit),0);
        if (target_coord.x < 0)
        {
            target_coord = new(AI.CalculatePossibleMapWithoutRow(torpedo_hit,true),0);
        }
        if (target_coord.x < 0)
        {
            goto End;
        }
        Vector2Int _direction = Vector2Int.up;
        Vector2Int size = PVEController.instance.size;
        Vector2Int coord = PVEController.instance.GetEdgeCoord(target_coord, _direction, size);
        List<Vector2Int> range = PVEController.instance.CurrentSkillRange;
        bool vertical = _direction.x == 0;
        for(int i=0;i<(vertical?size.y:size.x);i++)
        {
            bool hit = PVEController.instance.EnemyCheck(coord).Contains(ActionMessage.ActionResult.Hit);
            if(hit)break;
            PVEController.instance.EnemyAttack(coord);
            AI.Remove(coord);
            AI.UpdatePossibleMapAfterHit(coord, new KeyValuePair<int, LayoutDATA>(-1,new LayoutDATA(range,0)));
            coord += _direction;
        }
        messages = PVEController.instance.EnemyAttack(coord,skill.SkillRange);
        goto Update;
Radar:
        target_coord = AI.CalculateHuntMap(0.5f);
        goto Checkout;
Interference:
        target_coord = GetTarget(0.5f);
        goto Checkout;
Checkout:
        Dictionary<int,ActionMessage> message_dict = new();
        foreach(var v2 in skill.SkillRange)
        {
            Vector2Int target = target_coord + v2;
            ActionMessage message = PVEController.instance.EnemyCheck(target);
            if(message.Contains(ActionMessage.ActionResult.Hit))
            {
                if(!(message_dict.TryGetValue(message.ShipID,out var foreign_message)&&foreign_message.Locate == ActionMessage.ActionLocate.core))
                {
                    message_dict[message.ShipID] = message;
                }
                if(skill is radar) target_list.Enqueue(target);
                else if(skill is interference) PVEController.instance.PlayerLayoutMap.GetShip(message.Target).Buff.AddBuff(message.Locate == ActionMessage.ActionLocate.core ? EBuff.Interferenced_core : EBuff.Interferenced_body, 1);
            }
        }
        if(skill is radar) foreach(var kv in message_dict)
        {
            ActionMessage message = kv.Value;
            PVE_Notice.Create().ShowNotice_BeAction(message.ShipName, SPYON);
        }
        else if(skill is interference) foreach(var kv in message_dict)
        {
            ActionMessage message = kv.Value;
            string LOCATE = message.Locate == ActionMessage.ActionLocate.core ? CORE : BODY;
            PVE_Notice.Create().ShowNotice_BeInterference(message.ShipName, LOCATE);
        }
        goto End;
Update:
        foreach(var message in messages)
        {
            if (message.Contains(ActionMessage.ActionResult.Destroyed))
            {
                AI.UpdatePossibleMapAfterDestroy(message.Target, message.DestroyedShips, message.DestroyedShipAbsoluteLayout);
            }
            else
            {
                AI.UpdatePossibleMapAfterHit(message.Target, message.HitShips);
            }
            AI.Remove(message.Target);
        }
        PVEController.instance.DisposeMessage(messages);
        if(messages.Any(v=>v.Contains(ActionMessage.ActionResult.GameOver)))
        {
            PVE_Notice.Create().ShowNotice_Defeat();
            Sequence sequence=DOTween.Sequence();
            sequence.AppendInterval(2f);
            sequence.AppendCallback(()=>SettlePage.GetUIView().Defeat());
            return;
        }
        messages = PVEController.instance.DealMessage(messages);
        foreach(var message in messages)
        {
            MessageToInfo(message);
        }
        Debug.Log(AI.GetCurrentProbabilityMap());
        goto End;
End:
        return;
    }

    public void BuffNextRound()
    {
        foreach(var ship in enemy_ship.Values)
        {
            ship.Buff.NextRound();
        }
    }

    // 检查该点位该布局是否可行
    private bool CheckLayoutValid(Vector2Int center, LayoutDATA layout)
    {
        foreach (var coord in layout.ToList)
        {
            if (!available_map.Contains(center + coord))
                return false;
        }
        return true;
    }

    private Vector2Int GetTarget(float per=0.5f)
    {
        while(target_list.Count > 0)
        {
            Vector2Int coord = target_list.Dequeue();
            if (!AI.CheckAvailable(coord))
            {
                continue;
            }
            return coord;
        }
        return AI.CalculatePossibleMap(per);
    }

    public List<Vector3> CheckWholeShip(int ShipID)
    {
        List<Vector3> result = new();
        LayoutDATA layout = layout_map.GetAbsoluteLayout(ShipID);
        if (layout == null)
        {
            return result;
        }
        foreach(var coord in layout.ToList)
        {
            gridCellGroup_Enemy.Check(coord);
            result.Add(gridCellGroup_Enemy.GetGridCellPosition(coord));
        }
        return result;
    }

    public List<Vector2Int> GetWholeLine(Vector2Int center,Vector2Int direction)
    {
        List<Vector2Int> line = new();
        bool vertical = direction.x == 0;
        for(int i=0;i<(vertical?size.y:size.x);i++)
        {
            Vector2Int coord = vertical ? new Vector2Int(center.x, i) : new Vector2Int(i, center.y);
            line.Add(coord);
        }
        return line;
    }

    //

    private void MessageToInfo(ActionMessage message)
    {
        if (message.Contains(ActionMessage.ActionResult.Hit))
        {
            string LOCATE = message.Locate == ActionMessage.ActionLocate.core ? CORE : BODY;
            PVE_Notice.Create().ShowNotice_BeHit(message.ShipName, LOCATE);
        }
        else if (message.Contains(ActionMessage.ActionResult.Destroyed))
        {
            string ACTION;
            if(message.CoreDamaged)
            {
                ACTION = DESTROY;
            }
            else
            {
                ACTION = CAPTURE;
            }
            PVE_Notice.Create().ShowNotice_BeAction(message.ShipName, ACTION);
        }
    }
}
