using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// DataManager 管理工具
public class DataManagerChanger
{
    public ShipData_SO ShipData { set => DataManager.instance.shipData_dict = value.Sheet1.ToDictionary(v => v.uid, v => v); }
    
    public void Init()
    {
        DataManager.instance.Init();
    }
}

// DataManager 核心
public class DataManager : MonoBehaviour
{
    internal Dictionary<int, ShipData> shipData_dict;
    
    private static DataManager _dataManager;
    public static DataManager instance
    {
        get
        {
            if (!_dataManager)
            {
                _dataManager = FindObjectOfType(typeof(DataManager)) as DataManager;
                if (!_dataManager)
                    return null;
            }
            return _dataManager;
        }
    }

    internal void Init()
    {
        // TODO
    }

    public ShipData GetShipData(int id)
    {
        if (shipData_dict == null)
        {
            Debug.LogError("未初始化的数据库 CellCard_data");
            return null;
        }

        if (!shipData_dict.ContainsKey(id))
        {
            Debug.LogError($"未知的 ShipData ID : {id} ");
            return null;
        }

        return shipData_dict[id];
    }

    public Dictionary<int,string> GetShipUrlList()
    {
        return shipData_dict.ToDictionary(kv => kv.Key, kv => kv.Value.Url);
    }
}
