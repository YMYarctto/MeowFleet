using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Vector2Int size;
    public List<int> enemy_ships_id;

    List<int> target_ships_id;

    EventGroup _event;

    List<LayoutDATA> target_ship;
    List<Ship> enemy_ship;
    List<Skill> enemy_skill;

    List<Vector2Int> available_map;
    LayoutMap layout_map;

    public LayoutMap EnemyLayoutMap => layout_map;

    EnemyBehavior AI;

    public int EnemyShootCount => layout_map.AttackCount;

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

        int ShipID = 20000;
        enemy_ship = enemy_ships_id.ConvertAll(id =>
        {
            ShipID++;
            Ship ship = new Ship(ShipID,DataManager.instance.GetShipData(id));
            Skill skill = Skill.Get(ship,null);
            if (skill != null)
            {
                enemy_skill.Add(skill);
            }
            return ship;
        });

        MapInit();

        AI = new EnemyBehavior();
    }

    void Start()
    {
        target_ships_id = PVEController.instance.GetPlayerShipsID();
        target_ship = target_ships_id.ConvertAll(id =>
        {
            ShipData ship_data = DataManager.instance.GetShipData(id);
            return new LayoutDATA(ship_data.shape_coord,ship_data.core_number);
        });

        AI.Init(DataManager.instance.SaveData.MapSize, target_ship);
    }

    void OnEnable()
    {
        _event.AddListener(EventRegistry.PVE.EnemyTurn, EnemyTurn);
    }

    void OnDisable()
    {
        _event?.RemoveListener(EventRegistry.PVE.EnemyTurn, EnemyTurn);
    }

    private void MapInit()
    {
        available_map = new();
        layout_map = new();

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

            foreach (var ship in enemy_ship)
            {
                LayoutDATA layout = new(ship.Layout);
                for (int i = 0; i < 400; i++)
                {
                    System.Random rand = new();
                    LayoutDATA layout_ran = rand.Next(2) == 0 ? layout : new(layout.ToList.ConvertAll(coord => new Vector2Int(coord.y, coord.x)),layout.CoreNumber);
                    Vector2Int pos = available_map[rand.Next(available_map.Count)];
                    if (CheckLayoutValid(pos, layout_ran))
                    {
                        ship.ForceSetLayout(layout_ran);
                        layout_map.AddShip(pos, ship);
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
                    if (i == 399)
                    {
                        Debug.LogError("无法在地图中放置所有目标舰船");
                    }
                }
            }
        });
        task.Wait();
    }

    public void EnemyTurn()
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < enemy_skill.Count; i++)
        {
            if(!enemy_skill[i].CanSkill)continue;
            Skill skill = enemy_skill[i];
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(()=>ShipSkill(skill));
        }
        sequence.AppendInterval(1f);
        for (int i = 0; i < EnemyShootCount; i++)
        {
            sequence.AppendInterval(0.2f);
            sequence.AppendCallback(()=>Attack());
        }
        sequence.AppendInterval(1.5f);
        sequence.AppendCallback(()=>PVEController.instance.NextRound());
        sequence.Play();
    }

    public ActionMessage PlayerCheck(Vector2Int coord)
    {
        return layout_map.Checkout(coord);
    }

    public ActionMessage PlayerHit(Vector2Int coord)
    {
        return layout_map.GetMessage(coord);
    }

    public void DisposeMessage(List<ActionMessage> messages)
    {
        layout_map.DisposeMessage(messages);
    }
    
    private void Attack()
    {
        Vector2Int target_coord = AI.CalculatePossibleMap();
        FXManager.instance.AttackFX(PVEController.instance.GetPositionInScene(target_coord));
        ActionMessage message = PVEController.instance.EnemyAttack(target_coord);
        PVEController.instance.DisposeMessage(message);
        Debug.Log(message);
        if (message.Contains(ActionMessage.ActionResult.Miss) || message.Contains(ActionMessage.ActionResult.Hit))
        {
            AI.UpdatePossibleMapAfterHit(target_coord, message.HitShips);
        }
        else if (message.Contains(ActionMessage.ActionResult.Destroyed))
        {
            AI.UpdatePossibleMapAfterDestroy(target_coord, message.DestroyedShips);
        }

        AI.Remove(target_coord);
        Debug.Log(AI.GetCurrentProbabilityMap());

        if(message.Contains(ActionMessage.ActionResult.GameOver))
        {
            PVE_Notice.Create().ShowNotice_Defeat();
            Sequence sequence=DOTween.Sequence();
            sequence.AppendInterval(2f);
            sequence.AppendCallback(()=>UIManager.instance.GetUIView<SettlePage>().Defeat());
            return;
        }
    }

    private void ShipSkill(Skill skill)
    {
        if(skill is bomb)
        {
            goto Bomb;
        }else
        {
            goto End;
        }
Bomb:
        Vector2Int target_coord = AI.CalculatePossibleMap();
        List<Vector2Int> skill_range = AI.CalculateSkillRange(target_coord,new LayoutDATA(skill.SkillRange,0));
        
        List<ActionMessage> messages = PVEController.instance.EnemyAttack(target_coord,skill_range);
        PVEController.instance.DisposeMessage(messages);
        if(messages.Any(v=>v.Contains(ActionMessage.ActionResult.GameOver)))
        {
            PVE_Notice.Create().ShowNotice_Defeat();
            Sequence sequence=DOTween.Sequence();
            sequence.AppendInterval(2f);
            sequence.AppendCallback(()=>UIManager.instance.GetUIView<SettlePage>().Defeat());
            return;
        }
        foreach(var message in messages)
        {
            Debug.Log(message);
            if (message.Contains(ActionMessage.ActionResult.Miss) || message.Contains(ActionMessage.ActionResult.Hit))
            {
                AI.UpdatePossibleMapAfterHit(message.Target, message.HitShips);
            }
            else if (message.Contains(ActionMessage.ActionResult.Destroyed))
            {
                AI.UpdatePossibleMapAfterDestroy(message.Target, message.DestroyedShips);
            }
            AI.Remove(message.Target);
        }
        Debug.Log(AI.GetCurrentProbabilityMap());
End:
        return;
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
}
