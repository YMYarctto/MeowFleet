using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// DataManager 核心
public partial class DataManager : MonoBehaviour
{
    public SaveData_SO SaveData => saveData_SO;
    public AudioData_SO AudioData => audio_data;

    private Dictionary<int, ShipData> shipData_dict;
    private Dictionary<int, List<EnemyGroup>> enemyGroup_dict;
    private SaveData_SO saveData_SO;
    private AudioData_SO audio_data;
    
    private static DataManager _instance;
    public static DataManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(DataManager)) as DataManager;
                if (!_instance)
                    return null;
            }
            return _instance;
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

    public EnemyGroup RandomGetEnemyGroup(int layer)
    {
        if (enemyGroup_dict == null)
        {
            Debug.LogError("未初始化的数据库 EnemyGroup_data");
            return null;
        }

        if (!enemyGroup_dict.ContainsKey(layer))
        {
            Debug.LogError($"未知的 EnemyGroupData 层数 : {layer} ");
            return null;
        }

        //TODO 优化
        int ran = SeedController.instance.Range(0, enemyGroup_dict[layer].Sum(v => v.weight));
        List<EnemyGroup> group_list = enemyGroup_dict[layer];
        int weight = 0;
        foreach (var group in group_list)
        {
            weight += group.weight;
            if (weight > ran)
            {
                return group;
            }
        }
        return group_list[group_list.Count - 1];
    }

    public List<EnemyGroup> RandomGetEnemyGroupList(int layer)
    {
        if (enemyGroup_dict == null)
        {
            Debug.LogError("未初始化的数据库 EnemyGroup_data");
            return null;
        }

        if (!enemyGroup_dict.ContainsKey(layer))
        {
            Debug.LogError($"未知的 EnemyGroupData 层数 : {layer} ");
            return null;
        }

        List<EnemyGroup> result=new();
        foreach(var enemyGroup in enemyGroup_dict[layer])
        {
            for(int i = 0; i < enemyGroup.weight; i++)
            {
                result.Add(enemyGroup);
            }
        }
        Tools.Shuffle(result,SeedController.instance.Random);
        return result;
    }


    public Dictionary<int, string> GetShipUrlList()
    {
        return shipData_dict.ToDictionary(kv => kv.Key, kv => kv.Value.Url);
    }

    public string GetShipName(int id)
    {
        if (!shipData_dict.ContainsKey(id))
        {
            Debug.LogError($"未找到id: {id}");
            return "";
        }

        return shipData_dict[id].ship_name_string;
    }

    public static Changer GetDataManagerChanger()
    {
        return new Changer();
    }
    
    // DataManager 管理工具
    public class Changer
    {
        public ShipData_SO ShipData { set => instance.shipData_dict = value.Sheet1.ToDictionary(v => v.uid, v => v); }
        public EnemyGroup_SO EnemyGroupData { set => instance.enemyGroup_dict = value.Sheet1.GroupBy(v => v.layer).ToDictionary(g => g.Key, g => g.ToList()); }
        public SaveData_SO SaveData { set => instance.saveData_SO = value; }
        public AudioData_SO AudioData{set => instance.audio_data = value;}
        
        public void Init()
        {
            instance.Init();
        }
    }
}
