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
    Dictionary<string, Sprite> _sprites;

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
        _perfabs = new();
        _sprites = new();
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

    public Sprite GetSpriteByType(Type type)
    {
        if (ResourceList.skill_card_sprite.TryGetValue(type, out string url)&&_sprites.TryGetValue(url,out Sprite sprite))
        {
            return sprite;
        }
        return null;
    }

    public Sprite GetShipSprite(int id)
    {
        if (_sprites.TryGetValue(id.ToString(), out Sprite sprite))
        {
            return sprite;
        }
        Debug.LogError($"获取 \"{id}\" Texture2d 失败");
        return null;
    }

    // 初始化 DataManager
    IEnumerator InitDataManager()
    {
        LoadingPackage pkg = new(0);
        DataManager.DataManagerChanger dataManager = DataManager.GetDataManagerChanger();

        pkg.AddCount();
        Addressables.LoadAssetAsync<ShipData_SO>("ShipData_SO").Completed += (handle) =>
        {
            var so = handle.Result;
            so.Sheet1.ForEach(v =>
            {
                v.skill_name = Enum.Parse<Skill_Enum>(v.skill_name_string);
                v.shape_coord = JsonConvert.DeserializeObject<List<Vector2Int>>(v.shape_coord_string);
                v.skill_coord = JsonConvert.DeserializeObject<List<Vector2Int>>(v.skill_coord_string);
            });
            dataManager.ShipData = so;
            pkg.AddProgress();
        };

        pkg.AddCount();
        Addressables.LoadAssetAsync<SaveData_SO>("SaveData_SO").Completed += (handle) =>
        {
            var so = handle.Result;
            dataManager.SaveData = so;
            so.New();
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

        foreach (var kv in ResourceList.gameobjects)
        {
            pkg.AddCount();
            Addressables.LoadAssetAsync<GameObject>(kv.Value).Completed += (handle) =>
            {
                var obj = handle.Result;
                _perfabs[kv.Key] = obj;
                pkg.AddProgress();
            };
        }

        foreach (var kv in ResourceList.skill_card_sprite)
        {
            pkg.AddCount();
            Addressables.LoadAssetAsync<Texture2D>(kv.Value).Completed += (handle) =>
            {
                var t2d = handle.Result;
                Vector2 pivot = new(0.5f,0.5f);
                _sprites[kv.Value] = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height),pivot);
                pkg.AddProgress();
            };
        }

        foreach (var kv in DataManager.instance.GetShipUrlList())
        {
            pkg.AddCount();
            Addressables.LoadAssetAsync<Texture2D>(kv.Value).Completed += (handle) =>
            {
                var t2d = handle.Result;
                float ppu = 5f;
                Vector2 pivot = ResourceList.ships_sprite_pivot[kv.Value];
                _sprites[kv.Key.ToString()] = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), pivot, ppu);
                pkg.AddProgress();
            };
        }

        // 等待所有资源加载完成
        yield return new WaitUntil(() => pkg.Finish());
        Debug.Log("资源加载完成");
        IsInit = true;
        EventManager.instance.Init();
        SceneController.instance.Init();
        DOTween.Init();
        InputController.instance.Init();

        ShipManager.instance.Init();
        ShipManager.instance.Read();

        SceneController.instance.AfterSceneLoadAction(() =>
        {
            UIManager.instance.EnableUIView<BGAnimator_TitleScene>();
        });
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