using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public List<int> DataID;

    int max_id;
    Dictionary<int, Ship> shipDict;

    public Dictionary<int,Ship> Shiphouse => new(shipDict);

    private static ShipManager _instance;
    public static ShipManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(ShipManager)) as ShipManager;
                if (!_instance)
                {
                    Debug.LogError("场景中未找到 ShipManager");
                    return null;
                }
            }
            return _instance;
        }
    }

    public void Init()
    {
        shipDict = new();
    }

    public void Read()
    {
        max_id = DataManager.instance.SaveData.GetShiphouseData(out shipDict);
    }

    //TODO
    public void New()
    {
        shipDict.Clear();
        max_id = 10000;
        foreach(var data_id in DataID)
        {
            max_id++;
            Ship ship = new(max_id,DataManager.instance.GetShipData(data_id));
            AddShip(ship);
        }
    }

    public void AddShip(Ship ship)
    {
        max_id++;
        shipDict.Add(max_id, ship);
    }

    public Ship GetShip(int id)
    {
        if (!shipDict.ContainsKey(id))
        {
            Debug.LogError($"未知的 Ship ID : {id} ");
            return null;
        }
        return shipDict[id];
    }

    public Ship GetShip_Safe(int id)
    {
        if (!shipDict.ContainsKey(id))
        {
            return null;
        }
        return shipDict[id];
    }
}
