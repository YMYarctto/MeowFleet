using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    public abstract UIView currentView { get; }

    void Awake()
    {
        if (currentView == null)
        {
            Init();
            return;
        }
        UIManager.instance.AddUIView(currentView);
    }

    public abstract void Init();
    
    public virtual void Enable()=> gameObject.SetActive(true);
    public virtual void Disable()=> gameObject.SetActive(false);

    protected virtual void OnDestroy()
    {
        if (currentView == null)
        {
            return;
        }
        UIManager.instance?.RemoveUIView(currentView);
    }
    
}
