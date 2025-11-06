using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiphouseController : MonoBehaviour
{
    List<Transform> child_list;

    private static ShiphouseController _instance;
    public static ShiphouseController instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(ShiphouseController)) as ShiphouseController;
                if (!_instance)
                {
                    Debug.LogError("场景中未找到 ShiphouseController");
                    return null;
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        child_list = new List<Transform>();
        Transform shiphouse = GameObject.Find("Shiphouse").transform;
        for (int index = 0; index < shiphouse.childCount; index++)
        {
            child_list.Add(shiphouse.GetChild(index));
        }
        foreach(var id in DataManager.instance.GetShipUrlList().Keys)
        {
            GameObject obj = Instantiate(ResourceManager.instance.GetPerfabByType<Ship_UI>());
            obj.transform.SetParent(child_list[id], false);
            Ship_UI ship_ui = obj.GetComponent<Ship_UI>();
            ship_ui.Init(id);
        }
    }
}
