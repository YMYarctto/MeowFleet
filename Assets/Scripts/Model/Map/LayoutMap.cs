using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayoutMap
{
    Dictionary<int, LayoutDATA> _absolute_layout_map;// ship_id ->
    Dictionary<Vector2Int, ShipStatus> _ship_map;// -> ship_id
    Dictionary<Vector2Int, int> _status_map;// -> count
    Dictionary<int, int> _data_id;// ship_id -> dataId

    public int AttackCount => _absolute_layout_map.Count(kv => kv.Value.BodyList.Any(pos => _status_map.TryGetValue(pos, out int hp) && hp > 0));

    struct ShipStatus
    {
        public int ID;// map_id
        public Ship Ship;
        public LayoutDATA layout=>Ship.Layout;
    }

    public LayoutMap()
    {
        _absolute_layout_map = new();
        _ship_map = new();
        _status_map = new();
        _data_id = new();
    }

    public void AddShip(int ShipID,Vector2Int center, Ship ship)
    {
        int id = ShipID;
        LayoutDATA _layout = ship.Layout;
        ShipStatus ship_status = new ShipStatus { ID = id, Ship = ship };
        _data_id.Add(id, ship.DataId);
        _absolute_layout_map[id] = new(_layout.LayoutInMap(center),_layout.CoreNumber);
        foreach (var coord in _layout.ToList)
        {
            Vector2Int absolute_coord = center + coord;
            _ship_map[absolute_coord] = ship_status;
            _status_map[absolute_coord] = 1;
        }

        Debug.Log($"已添加舰船 ID: {ship.ShipId}");
    }

    public LayoutDATA Checkout(int ShipID)
    {
        if(!_absolute_layout_map.TryGetValue(ShipID,out LayoutDATA layout))
        {
            return null;
        }
        return layout;
    }

    public ActionMessage Checkout(Vector2Int target)
    {
        if (!_ship_map.ContainsKey(target))
        {
            return new ActionMessage(target,ActionMessage.ActionResult.Miss);
        }
        int _id = _ship_map[target].ID;
        int ship_id = _ship_map[target].Ship.ShipId;
        int data_id = _data_id[_id];
        ActionMessage message = new ActionMessage(data_id,ship_id,target,ActionMessage.ActionResult.Hit);
        if(!_absolute_layout_map[_id].BodyList.Contains(target))
        {
            message.SetLocate(ActionMessage.ActionLocate.core);
        }
        else
        {
            message.SetLocate(ActionMessage.ActionLocate.body);
        }
        return message;
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
        int ship_id = _ship_map[target].Ship.ShipId;
        int data_id = _data_id[_id];
        Vector2Int layout_coord = _ship_map[target].layout.ToList[_absolute_layout_map[_id].ToList.IndexOf(target)];
        _ship_map[target].Ship.Hit(layout_coord);
        if(!_absolute_layout_map[_id].BodyList.All(v=>_status_map[v]==0))
        {
            // Hit
            message = new ActionMessage(data_id,ship_id,target,ActionMessage.ActionResult.Hit);
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
        message = new ActionMessage(data_id,ship_id, target, ActionMessage.ActionResult.Destroyed);
        message.AddDestroyedShip(_id, _ship_map[target].layout);
        if (_absolute_layout_map[_id].CoreList.All(v => _status_map[v] != 0))
        {
            message.SetLocate(ActionMessage.ActionLocate.body);
        }
        else
        {
            message.SetLocate(ActionMessage.ActionLocate.core);
        }
        if(_status_map.All(kv=>kv.Value==0))
        {
            // GameOver
            message.AddResult(ActionMessage.ActionResult.GameOver);
        }

        return message;
    }

    public void DisposeMessage(ActionMessage message)
    {
        if(!_ship_map.ContainsKey(message.Target))return;
        int _id = _ship_map[message.Target].ID;
        if(message.Contains(ActionMessage.ActionResult.Destroyed))
        {
            _absolute_layout_map[_id].ToList.ForEach(v=>_status_map[v]=0);
        }
    }

    public void DisposeMessage(List<ActionMessage> messages)
    {
        for(int i=0;i< messages.Count;i++)
        {
            ActionMessage message = messages[i];
            if(!_ship_map.ContainsKey(message.Target))continue;
            int _id = _ship_map[message.Target].ID;
            if(message.Contains(ActionMessage.ActionResult.Destroyed))
            {
                _absolute_layout_map[_id].ToList.ForEach(v=>_status_map[v]=0);
                if(_status_map.All(kv=>kv.Value==0))
                {
                    // GameOver
                    message.AddResult(ActionMessage.ActionResult.GameOver);
                }
            }
        }
    }
}
