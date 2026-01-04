using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIView : MonoBehaviour
{
    public abstract UIView currentView { get; }
    public virtual int ID { get => 0; }
    protected virtual UnityAction WaitForAllUIViewAdded{get=>null;}

    void Awake()
    {
        if (currentView == null)
        {
            Init();
            return;
        }
        UIManager.instance.AddUIView(currentView,ID);
        if(WaitForAllUIViewAdded!=null)
        {
            StartCoroutine(EWaitForAllUIViewAdded());
        }
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
        UIManager.instance?.RemoveUIView(currentView,ID);
    }
    
    IEnumerator EWaitForAllUIViewAdded()
    {
        yield return new WaitForEndOfFrame();
        WaitForAllUIViewAdded?.Invoke();
    }
}
