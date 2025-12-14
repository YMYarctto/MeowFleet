using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPool<T> where T:FX
{
    private readonly Stack<T> pool = new();
    private readonly T prefab;
    private readonly Transform parent;

    public FXPool(int prewarm, Transform parent)
    {
        prefab = ResourceManager.instance.GetPerfabByType<T>().GetComponent<T>();
        this.parent = parent;

        for (int i = 0; i < prewarm; i++)
        {
            var obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Push(obj);
        }
    }

    public T Get(Vector3 pos)
    {
        T obj;
        if (pool.Count > 0)
        {
            obj = pool.Pop();
            obj.transform.position = pos;
            obj.gameObject.SetActive(true);
            obj.OnComplete(()=>Release(obj));
            return obj;
        }

        obj = Object.Instantiate(prefab, parent);
        obj.transform.position = pos;
        obj.OnComplete(()=>Release(obj));

        return obj;
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Push(obj);
    }
}
