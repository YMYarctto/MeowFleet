using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    int max_id;
    Dictionary<int, Ship> shipDict;

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
}
