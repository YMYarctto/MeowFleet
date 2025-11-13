using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayoutMap
{
    Dictionary<int, List<Vector2Int>> _absolute_layout_map;
    Dictionary<Vector2Int, ShipStatus> _ship_map;
    Dictionary<Vector2Int, int> _status_map;
    int id = 0;

    public int Count => _absolute_layout_map.Count(kv => kv.Value.Any(pos => _status_map.TryGetValue(pos, out int hp) && hp > 0));

    struct ShipStatus
    {
        public int id;
        public LayoutDATA layout;
    }

    public LayoutMap()
    {
        _absolute_layout_map = new();
        _ship_map = new();
        _status_map = new();
    }

    public void AddShip(Vector2Int center, LayoutDATA layout)
    {
        id++;
        AddShip(id, center, layout);
    }

    public void AddShip(int _id,Vector2Int center, LayoutDATA _layout)
    {
        ShipStatus ship_status = new ShipStatus { id = _id, layout = _layout };
        _absolute_layout_map[_id] = _layout.LayoutInMap(center);
        foreach (var coord in _layout.ToList)
        {
            Vector2Int absolute_coord = center + coord;
            _ship_map[absolute_coord] = ship_status;
            _status_map[absolute_coord] = 1;
        }

        Debug.Log($"已添加舰船 ID: {_id}");
    }

    public ActionMessage GetMessage(Vector2Int target)
    {
        if (!_ship_map.ContainsKey(target))
        {
            // Miss
            return new ActionMessage(target,ActionMessage.ActionResult.Miss);
        }

        ActionMessage message;
        _status_map[target] = 0;
        if(!_absolute_layout_map[_ship_map[target].id].All(v=>_status_map[v]==0))
        {
            // Hit
            message = new ActionMessage(target,ActionMessage.ActionResult.Hit);
            message.AddHitShip(_ship_map[target].id, _ship_map[target].layout);
            return message;
        }
        
        // Destroyed
        message = new ActionMessage(target,ActionMessage.ActionResult.Destroyed);
        message.AddDestroyedShip(_ship_map[target].id, _ship_map[target].layout);

        return message;
    }

}
