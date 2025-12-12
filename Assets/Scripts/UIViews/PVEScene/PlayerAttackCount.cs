using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttackCount : UIView
{
    public override UIView currentView => this;

    TMP_Text count;
    CanvasGroup image;

    Tween tween;

    public override void Init()
    {
        count = transform.Find("count").GetComponent<TMP_Text>();
        image = GetComponent<CanvasGroup>();
        image.alpha = 0;
    }

    public override void Enable()
    {
        tween?.Kill();
        tween = image.DOFade(1, 0.3f).SetEase(Ease.InQuad);
    }

    public override void Disable()
    {
        tween?.Kill();
        tween = image.DOFade(0, 0.3f).SetEase(Ease.OutQuad);
    }

    public void SetCount(int current)
    {
        count.text = $"{current}";
    }
}
