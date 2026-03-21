using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pool<T> where T:UIView
{
    private readonly Stack<T> pool = new();
    private readonly List<T> active = new();
    private readonly T prefab;
    private readonly Transform parent;
    private UnityAction actionWhenScaledUp;

    public Pool(int prewarm, Transform parent,UnityAction action = null)
    {
        prefab = ResourceManager.instance.GetPerfabByType<T>().GetComponent<T>();
        this.parent = parent;
        actionWhenScaledUp = action;

        for (int i = 0; i < prewarm; i++)
        {
            actionWhenScaledUp?.Invoke();
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
            active.Add(obj);
            return obj;
        }

        actionWhenScaledUp?.Invoke();
        obj = Object.Instantiate(prefab, parent);
        obj.transform.position = pos;
        active.Add(obj);
        
        return obj;
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Push(obj);
    }

    public void ReleaseAll()
    {
        foreach (var obj in active)
        {
            if (obj != null)
            {
                Release(obj);
            }
        }
        active.Clear();
    }
}
