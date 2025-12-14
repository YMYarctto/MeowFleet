using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    private FXPool<FX_bomb2> pool_bomb2;
    Transform FX_Group;

    private static FXManager _instance;
    public static FXManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(FXManager)) as FXManager;
                if (!_instance)
                {
                    Debug.LogError("场景中未找到 FXManager");
                    return null;
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        FX_Group = GameObject.Find("FX_Group").transform;
        pool_bomb2 = new(30,FX_Group);
    }

    public FX_bomb2 AttackFX(Vector3 pos)
    {
        return pool_bomb2.Get(pos);
    }
}
