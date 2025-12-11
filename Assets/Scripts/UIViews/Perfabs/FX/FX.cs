using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public abstract class FX : MonoBehaviour
{
    Sequence sequence;
    UnityAction onComplete;

    public void DestroySelf_Delay(float delay)
    {
        sequence = DOTween.Sequence();
        sequence.AppendInterval(delay);
        sequence.AppendCallback(()=>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
        sequence.Play();
    }

    public void OnComplete(UnityAction action)
    {
        onComplete = action;
    }

    public static FX Create<T>(Vector3 position,Transform parent)where T : FX
    {
        GameObject go = Instantiate(ResourceManager.instance.GetPerfabByType<T>(), parent,false);
        go.transform.position = position;
        return go.GetComponent<FX>();
    }
}
