using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayoutMap
{
    Dictionary<int, LayoutDATA> _absolute_layout_map;// map_id ->
    Dictionary<Vector2Int, ShipStatus> _ship_map;// -> map_id
    Dictionary<Vector2Int, int> _status_map;// -> count
    Dictionary<int, int> _ship_id;// map_id -> uid
    int id = 0;

    public int AttackCount => _absolute_layout_map.Count(kv => kv.Value.BodyList.Any(pos => _status_map.TryGetValue(pos, out int hp) && hp > 0));

    struct ShipStatus
    {
        public int ID;// map_id
        public LayoutDATA layout;
    }

    public LayoutMap()
    {
        _absolute_layout_map = new();
        _ship_map = new();
        _status_map = new();
        _ship_id = new();
    }

    public void AddShip(int ship_id,Vector2Int center, LayoutDATA _layout)
    {
        id++;
        ShipStatus ship_status = new ShipStatus { ID = id, layout = _layout };
        _ship_id.Add(id, ship_id);
        _absolute_layout_map[id] = new(_layout.LayoutInMap(center),_layout.CoreNumber);
        foreach (var coord in _layout.ToList)
        {
            Vector2Int absolute_coord = center + coord;
            _ship_map[absolute_coord] = ship_status;
            _status_map[absolute_coord] = 1;
        }

        Debug.Log($"已添加舰船 ID: {ship_id}");
    }

    public ActionMessage GetMessage(Vector2Int target)
    {
        if (!_ship_map.ContainsKey(target))
        {
            // Miss
            return new ActionMessage(target,ActionMessage.ActionResult.Miss);
        }
        if(_status_map[target] == 0)
        {
            return new ActionMessage(target,ActionMessage.ActionResult.Miss);
        }

        ActionMessage message;
        _status_map[target]--;
        int _id = _ship_map[target].ID;
        int ship_id = _ship_id[_id];
        ShipManager.instance.GetShip_Safe(ship_id)?.Hit(target);
        if(!_absolute_layout_map[_id].BodyList.All(v=>_status_map[v]==0))
        {
            // Hit
            message = new ActionMessage(ship_id,target,ActionMessage.ActionResult.Hit);
            message.AddHitShip(_id, _ship_map[target].layout);

            if (_absolute_layout_map[_id].CoreList.Contains(target))
            {
                message.SetLocate(ActionMessage.ActionLocate.core);
            }
            else
            {
                message.SetLocate(ActionMessage.ActionLocate.body);
            }
            
            return message;
        }

        // Destroyed
        message = new ActionMessage(ship_id, target, ActionMessage.ActionResult.Destroyed);
        message.AddDestroyedShip(_id, _ship_map[target].layout);
        if (_absolute_layout_map[_id].CoreList.All(v => _status_map[v] != 0))
        {
            message.SetLocate(ActionMessage.ActionLocate.body);
        }
        else
        {
            message.SetLocate(ActionMessage.ActionLocate.core);
        }
        _absolute_layout_map[_id].ToList.ForEach(v=>_status_map[v]=0);
        if(_status_map.All(kv=>kv.Value==0))
        {
            // GameOver
            message.AddResult(ActionMessage.ActionResult.GameOver);
        }

        return message;
    }

}
