using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSelectController : MonoBehaviour
{
    public static int AreaID = 0;
    public Transform UI_trans { get; private set; }
    public GameObject PointerPrefab { get; private set; }

    Transform bg_trans;

    private static AreaSelectController _instance;
    public static AreaSelectController instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(AreaSelectController)) as AreaSelectController;
                if (!_instance)
                {
                    Debug.LogError("场景中未找到 AreaSelectController");
                    return null;
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        bg_trans = GameObject.Find("sea_bg").transform;
        UI_trans = GameObject.Find("UI").transform;
        PointerPrefab = ResourceManager.instance.GetPerfabByType<Pointer_Animation>();
        for (int i = 0; i < bg_trans.childCount; i++)
        {
            AreaPart areaPage = bg_trans.GetChild(i).gameObject.AddComponent<AreaPart>();
            AreaID++;
        }
    }
}
