using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ship
{
    int id;
    ShipData shipData;
    LayoutDATA layout;
    LayoutDATA init_layout;
    int _direction = 0;
    List<int> damage_condition;

    List<int> core_condition=>damage_condition.GetRange(0,shipData.core_number);
    List<int> body_condition=>damage_condition.GetRange(shipData.core_number,damage_condition.Count-shipData.core_number);
    
    public LayoutDATA Layout => layout;
    public LayoutDATA InitLayout => init_layout;
    public int DataId => shipData.uid;
    public int ShipId => id;
    public int Direction => _direction;
    public string Name => shipData.ship_name_string;
    public Skill_Enum Skill => shipData.skill_name;
    public List<Vector2Int> SkillRange => new(shipData.skill_coord);
    public Status ShipStatus
    {
        get
        {
            if(core_condition.All(v=>v!=0)&&body_condition.All(v=>v!=0))
            {
                return Status.Intact;
            }
            else if(core_condition.All(v=>v!=0)&&body_condition.All(v=>v==0))
            {
                return Status.Capture;
            }
            else if(core_condition.All(v=>v==0)&&body_condition.Any(v=>v!=0))
            {
                return Status.CoreDamage;
            }
            else if (core_condition.Any(v=>v==0)&&body_condition.All(v=>v==0))
            {
                return Status.Destroyed;
            }
            else
            {
                return Status.Damage;
            }
        }
    }

    public Ship(int id,ShipData shipData)
    {
        this.id = id;
        this.shipData = shipData;
        init_layout = layout = new(shipData.shape_coord,shipData.core_number);
        damage_condition = init_layout.ToList.ConvertAll(v=>1);
    }

    public int ResetLayout()
    {
        layout = new(init_layout);
        int value = _direction;
        _direction = 0;
        return value;
    }

    public void Rotate(int direction)
    {
        layout = layout.Rotate(direction);
        _direction += direction;
    }

    public void Hit(Vector2Int coord)
    {
        int index = layout.ToList.IndexOf(coord);
        if(index<0||damage_condition[index]<=0)
        {
            return;
        }
        damage_condition[index]--;
        Debug.Log($"Ship {id} 受到攻击, 当前状态: {ShipStatus}");
    }

    public enum Status
    {
        Intact,
        Damage,
        CoreDamage,
        Destroyed,
        Capture,
    }
}
