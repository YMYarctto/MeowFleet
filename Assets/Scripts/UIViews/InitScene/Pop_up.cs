using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Pop_up : UIView<Pop_up>
{
    TMP_Text content;
    Transform bg;
    Tween tween;

    public override void Init()
    {
        bg = transform.Find("bg");
        content = bg.Find("content").GetComponent<TMP_Text>();
        bg.localScale = Vector3.zero;
        base.Disable();
    }

    public void Show(string text)
    {
        content.text = text;
        Enable();
    }

    public override void Enable()
    {
        base.Enable();
        tween?.Kill();
        bg.localScale = Vector3.zero;
        tween = bg.DOScale(1, 0.1f).SetEase(Ease.OutBack);
    }

    public override void Disable()
    {
        tween?.Kill();
        tween = bg.DOScale(0, 0.1f).SetEase(Ease.InQuad).OnComplete(() => base.Disable());
    }
}
