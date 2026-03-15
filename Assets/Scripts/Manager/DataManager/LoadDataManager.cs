using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// LoadDataManager 管理游戏中的需加载资源
public partial class LoadDataManager : MonoBehaviour
{
    public bool IsInit { get; private set; } = false;

    public PVELoadData_SO PVELoadData => pve_data_so;

    private PVELoadData_SO pve_data_so;
    
    private static LoadDataManager _instance;
    public static LoadDataManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(LoadDataManager)) as LoadDataManager;
                if (!_instance)
                    return null;
            }
            return _instance;
        }
    }

    internal void Init()
    {
        IsInit = true;
    }

    public static Changer GetDataManagerChanger()
    {
        return new Changer();
    }
    
    // 管理工具
    public class Changer
    {
        public PVELoadData_SO PVEData_SO{set => instance.pve_data_so = value;}
        
        public void Init()
        {
            instance.Init();
        }
    }
}
