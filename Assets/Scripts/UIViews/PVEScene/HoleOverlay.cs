using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HoleOverlay : UIView
{
    public override UIView currentView => this;

    public FXPool<FX_holeWriter> fx_pool;

    Transform Reader;
    Transform Writer;
    CanvasGroup canvasGroup;
    Tween tween;

    bool visible = false;

    public override void Init()
    {
        Reader = transform.Find("Reader");
        Writer = transform.Find("WriterGroup");
        fx_pool = new(50,Writer,FXKind.ManualRelease);
        canvasGroup = Reader.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void ShowOverlay(Vector3[] target_holes)
    {
        if (visible) return;
        visible = true;
        tween?.Kill();
        tween = canvasGroup.DOFade(1f,0.15f).SetEase(Ease.OutQuad);
        foreach (var v3 in target_holes)
        {
            fx_pool.Get(v3);
        }
    }

    public void ClearOverlay()
    {
        if (!visible) return;
        visible = false;
        tween?.Kill();
        tween = canvasGroup.DOFade(0f,0.15f).SetEase(Ease.OutQuad);
        fx_pool.ReleaseAll();
    }

}
