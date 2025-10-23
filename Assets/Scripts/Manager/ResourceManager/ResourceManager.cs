using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager : MonoBehaviour
{
    // 是否初始化完成
    public static bool IsInit{ get; private set; } = false;

    Dictionary<Type, GameObject> _perfabs;

    private static ResourceManager _resourceManager;
    public static ResourceManager instance
    {
        get
        {
            if (!_resourceManager)
            {
                _resourceManager = FindObjectOfType(typeof(ResourceManager)) as ResourceManager;
                if (!_resourceManager)
                    return null;
            }
            if (!IsInit)
            {
                Debug.LogWarning("尝试访问未完成初始化的 ResourceManager");
                return null;
            }
            return _resourceManager;
        }
    }

    void Awake()
    {
        StartCoroutine(InitDataManager());
    }

    public GameObject GetPerfabByType<T>() where T : MonoBehaviour
    {
        Type type = typeof(T);
        if (_perfabs.TryGetValue(type, out GameObject obj))
        {
            return obj;
        }
        Debug.LogError($"获取 \"{type}\" 预制体失败");
        return null;
    }

    // 初始化 DataManager
    IEnumerator InitDataManager()
    {
        LoadingPackage pkg = new(0);
        DataManagerChanger dataManager = new();
        pkg.AddCount();
        Addressables.LoadAssetAsync<ShipData_SO>("ShipData_SO").Completed += (handle) =>
        {
            var so = handle.Result;
            so.Sheet1.ForEach(v =>
            {
                v.ship_name = Enum.Parse<Ships_Enum>(v.ship_name_string);
                v.shape_coord = JsonConvert.DeserializeObject<List<Vector2Int>>(v.shape_coord_string);
            });
            dataManager.ShipData = so;
            pkg.AddProgress();
        };

        // 等待所有资源初始化完成
        yield return new WaitUntil(() => pkg.Finish());
        Debug.Log("游戏初始化成功");
        dataManager.Init();
        StartCoroutine(LoadResource());
    }

    // 资源加载
    IEnumerator LoadResource()
    {
        LoadingPackage pkg = new(0);

        // _perfabs = new();
        // foreach (var kv in ResourceList.gamobjects)
        // {
        //     pkg.AddCount();
        //     Addressables.LoadAssetAsync<GameObject>(kv.Value).Completed += (handle) =>
        //     {
        //         var obj = handle.Result;
        //         _perfabs[kv.Key] = obj;
        //         pkg.AddProgress();
        //     };
        // }

        // 等待所有资源加载完成
        yield return new WaitUntil(() => pkg.Finish());
        Debug.Log("资源加载完成");
        IsInit = true;
        EventManager.instance.Init();
        SceneController.instance.Init();
        DOTween.Init();

        SceneController.instance.ChangeScene(SceneRegistry.TitleScene);
    }

    // 数据包计数器
    struct LoadingPackage
    {
        int count;
        int progress;

        public LoadingPackage(int index)
        {
            count = index;
            progress = index;
        }

        public void AddCount(int count)
        {
            this.count += count;
        }

        public void AddCount()
        {
            count++;
        }

        public void AddProgress()
        {
            progress++;
        }

        public bool Finish()
        {
            return count == progress;
        }
    }
}