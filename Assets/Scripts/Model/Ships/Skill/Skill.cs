using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Skill
{
    protected List<Vector2Int> skill_coord;
    protected bool on_enemy_map;
    protected Ship ship;
    protected SkillCard_UI ui;
    protected Vector2Int _direction=new(0,1);

    public List<Vector2Int> SkillRange=>new(skill_coord);
    public PVEController.PVEMap TargetMap =>on_enemy_map?PVEController.PVEMap.Enemy:PVEController.PVEMap.Player;
    public Vector2Int Direction => _direction;

    public bool CanSkill=>ship.ShipStatus==Ship.Status.Intact||ship.ShipStatus==Ship.Status.Damage;

    public virtual void OnSelect()
    {
        _direction = new(0, 1);
        PVEController.instance.SetSkill(this);
    }

    public void OnSkill(Vector2Int target)
    {
        OnSkillInvoke(target);
        ui.OnSkillEnd();
        PVEController.instance.ClearSelectedSkill();
    }

    public void Rotate(int direction)
    {
        _direction = direction switch
        {
            1 => new Vector2Int(_direction.y, -_direction.x),
            -1 => new Vector2Int(-_direction.y, _direction.x),
            _ => _direction,
        };
    }

    public abstract void OnSkillInvoke(Vector2Int target);

    public static Skill Get(Ship ship,SkillCard_UI ui)
    {
        Skill_Enum skill = ship.Skill;
        Skill _this = skill switch
        {
            Skill_Enum.radar => null,
            Skill_Enum.interference => null,
            Skill_Enum.bomb_focus => new bomb_focus(),
            Skill_Enum.bomb_wide => new bomb_wide(),
            Skill_Enum.repair => null,
            Skill_Enum.shield => null,
            Skill_Enum.bait => null,
            Skill_Enum.fort => null,
            Skill_Enum.torpedo => new torpedo(),
            _=>null,
        };
        if (_this == null)
        {
            Debug.LogError("位置类型的舰船");
            return null;
        }
        _this.ui = ui;
        _this.ship = ship;
        _this.skill_coord = new(ship.SkillRange);
        _this.on_enemy_map = skill switch
        {
            Skill_Enum.radar => true,
            Skill_Enum.interference => true,
            Skill_Enum.bomb_focus => true,
            Skill_Enum.bomb_wide => true,
            Skill_Enum.repair => false,
            Skill_Enum.shield => false,
            Skill_Enum.bait => false,
            Skill_Enum.fort => false,
            Skill_Enum.torpedo => true,
            _ => true,
        };
        return _this;
    }
}
