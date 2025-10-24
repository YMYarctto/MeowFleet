using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SceneLoader : UIView
{
    public override UIView currentView => this;

    public UnityAction ActionAfterUnload;
    public UnityAction ActionAfterLoad;

    Image image;
    Color color;

    public override void Init()
    {
        image = GetComponent<Image>();
        color = image.color;
    }

    public override void Enable()
    {
        base.Enable();
        Close();
    }

    public override void Disable()
    {
        Open();
    }

    void Open()
    {
        DOTween.To(() => color.a, x => color.a=x, 0f, 1).SetEase(Ease.OutQuad).OnUpdate(()=>{
            image.color = color;
        }).OnComplete(OnOpenFinished);
    }

    void Close()
    {
        DOTween.To(() => color.a, x => color.a=x, 1f, 1).SetEase(Ease.InQuad).OnUpdate(()=>{
            image.color = color;
        }).OnComplete(OnCloseFinished);
    }

    public void OnOpenFinished()
    {
        base.Disable();
        UIManager.instance.DisableUIView<InteractionBarrier>();
        ActionAfterLoad?.Invoke();
        ActionAfterLoad = null;
    }

    public void OnCloseFinished()
    {
        SceneController.instance.StartChangeSceneCoroutine();
        ActionAfterUnload?.Invoke();
        ActionAfterUnload = null;
    }
}
