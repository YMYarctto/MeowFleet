using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Dictionary<Type,UIView> _UIs;

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

    public void AddUIView(UIView view)
    {
        if (view == null)
        {
            return;
        }
        Type url = view.GetType();
        if (_UIs.ContainsKey(url))
        {
            Debug.LogError($"UIManager:\n添加Url: \"{url}\" 失败,该Url已存在");
            return;
        }
        _UIs.Add(url, view);
        view.Init();

    }
    public void RemoveUIView(params UIView[] urls)
    {
        foreach(var u in urls){
            _UIs.Remove(u.GetType());
        }
    }

    public T GetUIView<T>()where T:UIView
    {
        Type url = typeof(T);
        if (!_UIs.ContainsKey(url))
        {
            Debug.LogError($"UIManager:\n获取Url: \"{url}\" 失败,该Url不存在");
            return null;
        }
        return _UIs[url].gameObject.GetComponent<T>();
    }

    public void EnableUIView<T>()where T:UIView
    {
        Type url = typeof(T);
        if (!_UIs.ContainsKey(url))
        {
            Debug.LogError($"UIManager:\n启用Url: \"{url}\" 失败,该Url不存在");
            return;
        }
        _UIs[url].Enable();
    }
    
    public void DisableUIView<T>() where T : UIView
    {
        Type url = typeof(T);
        if (!_UIs.ContainsKey(url))
        {
            Debug.LogError($"UIManager:\n禁用Url: \"{url}\" 失败,该Url不存在");
            return;
        }
        _UIs[url].Disable();
    }
}