using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMessage
{
    int data_id;
    int ship_id;
    Vector2Int _target;
    List<ActionResult> _result;
    KeyValuePair<int, LayoutDATA> _hit_ships;
    KeyValuePair<int, LayoutDATA> _destroyed_ships;
    ActionLocate _locate;

    public int ShipID => ship_id;
    public Vector2Int Target => _target;
    public ActionResult ResultFirst => _result[0];

    public string ShipName => DataManager.instance.GetShipName(data_id);

    public KeyValuePair<int, LayoutDATA> HitShips
    {
        get => _hit_ships;
    }
    public KeyValuePair<int, LayoutDATA> DestroyedShips
    {
        get => _destroyed_ships;
    }
    public ActionLocate Locate => _locate;

    public ActionMessage(int data,int ship, Vector2Int target, ActionResult result)
    {
        data_id = data;
        ship_id = ship;
        _target = target;
        _result = new() { result };
        _hit_ships = new(-1, null);
        _destroyed_ships = new(-1, null);
    }
    
    public ActionMessage(Vector2Int target,ActionResult result)
    {
        data_id = 0;
        _target = target;
        _result = new() { result };
        _hit_ships = new(-1,null);
        _destroyed_ships = new(-1,null);
    }

    public bool Contains(ActionResult result)
    {
        return _result.Contains(result);
    }

    public void AddHitShip(int id, LayoutDATA layout)
    {
        _hit_ships = new(id, layout);
    }

    public void AddDestroyedShip(int id, LayoutDATA layout)
    {
        _destroyed_ships = new(id, layout);
    }

    public void SetLocate(ActionLocate locate)
    {
        _locate = locate;
    }

    public void AddResult(ActionResult result)
    {
        _result.Add(result);
    }

    public override string ToString()
    {
        string str = "ActionResult:";
        foreach (var result in _result)
        {
            str += $"{result}; ";
        }
        str += $"\nTarget: ({_target.x},{_target.y})\n";
        str += $"Hit: {_hit_ships.Key} : {_hit_ships.Value}\n";
        str += $"Destroy: {_destroyed_ships.Key} : {_destroyed_ships.Value}\n";
        
        return str;
    }

    public enum ActionResult 
    {
        Miss,
        Hit,
        Destroyed,
        // Destroyed Body => Capture
        GameOver,
    }

    public enum ActionLocate
    {
        core,
        body,
    }
}
