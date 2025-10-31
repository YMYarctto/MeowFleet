using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Dictionary<(Type,int),UIView> _UIs;

    private static UIManager _UIManager;
    public static UIManager instance
    {
        get
        {
            if (!_UIManager)
            {
                _UIManager = FindObjectOfType(typeof(UIManager)) as UIManager;
                if (!_UIManager)
                {
                    return null;
                }
                _UIManager.Init();
            }
            return _UIManager;
        }
    }
    public void Init()
    {
        _UIs ??= new();
    }

    public void AddUIView(UIView view,int id)
    {
        if (view == null)
        {
            return;
        }
        Type url = view.GetType();
        if (_UIs.ContainsKey((url,id)))
        {
            Debug.LogError($"UIManager:\n添加Url: \"{url},\" ID: {id} 失败,该Url已存在");
            return;
        }
        _UIs.Add((url,id), view);
        view.Init();

    }
    public void RemoveUIView(int id,params UIView[] urls)
    {
        foreach(var u in urls){
            _UIs.Remove((u.GetType(),id));
        }
    }

    public T GetUIView<T>() where T : UIView
    {
        return GetUIView<T>(0);
    }

    public T GetUIView<T>(int id) where T : UIView
    {
        Type url = typeof(T);
        if (!_UIs.ContainsKey((url, id)))
        {
            Debug.LogError($"UIManager:\n获取Url: \"{url}\" 失败,该Url不存在");
            return null;
        }
        return _UIs[(url, id)].gameObject.GetComponent<T>();
    }
    
    public void EnableUIView<T>()where T:UIView
    {
        EnableUIView<T>(0);
    }

    public void EnableUIView<T>(int id) where T : UIView
    {
        Type url = typeof(T);
        if (!_UIs.ContainsKey((url, id)))
        {
            Debug.LogError($"UIManager:\n启用Url: \"{url}\" 失败,该Url不存在");
            return;
        }
        _UIs[(url, id)].Enable();
    }
    
    public void DisableUIView<T>() where T : UIView
    {
        DisableUIView<T>(0);
    }
    
    public void DisableUIView<T>(int id) where T : UIView
    {
        Type url = typeof(T);
        if (!_UIs.ContainsKey((url,id)))
        {
            Debug.LogError($"UIManager:\n禁用Url: \"{url}\" 失败,该Url不存在");
            return;
        }
        _UIs[(url,id)].Disable();
    }
}