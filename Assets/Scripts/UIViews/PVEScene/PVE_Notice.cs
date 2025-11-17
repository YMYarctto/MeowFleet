using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PVE_Notice : UIView
{
    public override UIView currentView => this;

    float MaxSize=2.5f;

    TMP_Text text;
    Transform bg;
    Transform bar_trans;
    CanvasGroup image;

    Sequence sequence;

    public override void Init()
    {
        bg = transform.Find("bg");
        bar_trans = transform.Find("bar");
        text = bar_trans.GetComponentInChildren<TMP_Text>();
        image = bar_trans.GetComponent<CanvasGroup>();
        Disable();
    }

    public void ShowNotice(string ship_str,string coord)
    {
        text.text = $"你命中了 [ {ship_str} ] 的{coord}";
        Enable();
    }

    public override void Enable()
    {
        Disable();
        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, 0.2f).SetEase(Ease.InQuad));
        sequence.Join(bar_trans.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.InQuad));
        sequence.Join(bg.DOScaleY(1f, 0.2f).SetEase(Ease.InQuad));
        sequence.AppendInterval(1.2f);
        sequence.Append(image.DOFade(0, 0.2f).SetEase(Ease.OutQuad));
        sequence.Join(bg.DOScaleY(0, 0.2f).SetEase(Ease.OutQuad));
    }

    public override void Disable()
    {
        image.alpha = 0;
        bar_trans.localScale = MaxSize*new Vector3(1f, 1f, 1f);
        bg.localScale = new(1f, 0, 1f);
    }

}
