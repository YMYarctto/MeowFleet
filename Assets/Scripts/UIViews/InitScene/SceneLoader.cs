using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;

public class SceneLoader : UIView
{
    public override UIView currentView => this;

    public UnityAction ActionAfterUnload;
    public UnityAction ActionAfterLoad;

    // RectTransform left;
    // RectTransform right;
    // RectTransform center;

    public override void Init()
    {
        // left = transform.Find("bg_left").GetComponent<RectTransform>();
        // right = transform.Find("bg_right").GetComponent<RectTransform>();
        // center = transform.Find("9_center").GetComponent<RectTransform>();
    }

    public override void Enable()
    {
        Close();
    }

    public override void Disable()
    {
        Open();
    }

    void Open()
    {
        Sequence loopTween_Open = DOTween.Sequence();
        // loopTween_Open.Append(left.DOLocalMove(new Vector3(-2199, 0, 0), 0.5f).SetEase(Ease.InQuad));
        // loopTween_Open.Join(right.DOLocalMove(new Vector3(2199, 0, 0), 0.5f).SetEase(Ease.InQuad));
        // loopTween_Open.Join(center.DOLocalMove(new Vector3(1239, 0, 0), 0.5f).SetEase(Ease.InQuad));
        loopTween_Open.InsertCallback(0.4f, OnOpenFinished);
        loopTween_Open.Play();
    }

    void Close()
    {
        base.Enable();//delete
        UIManager.instance.EnableUIView<InteractionBarrier>();
        // center.localScale = new Vector3(0.6f, 0.6f, 1f);
        // center.localRotation = Quaternion.Euler(0, 0, -90);
        Sequence loopTween_Close = DOTween.Sequence();
        // loopTween_Close.Append(left.DOLocalMove(new Vector3(-960, 0, 0), 1f).SetEase(Ease.InOutQuad));
        // loopTween_Close.Join(right.DOLocalMove(new Vector3(960, 0, 0), 1f).SetEase(Ease.InOutQuad));
        // loopTween_Close.Join(center.DOLocalMove(new Vector3(300f, 0, 0), 0.1f).SetDelay(0.8f));
        // loopTween_Close.Append(center.DOLocalMove(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutQuad));
        // loopTween_Close.Join(center.DORotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutQuad));
        // loopTween_Close.Join(center.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InOutQuad));
        // loopTween_Close.AppendInterval(0.5f);
        loopTween_Close.AppendCallback(OnCloseFinished);
        loopTween_Close.Play();
    }

    public void OnOpenFinished()
    {
        base.Disable();//delete
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
