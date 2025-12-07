using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class Aim : UIView
{
    public override UIView currentView => this;

    Image image;

    Tween active_tween;
    Tween move_tween;
    bool active;

    public override void Init()
    {
        image = GetComponent<Image>();
        image.color = new Color(1, 1, 1, 0);
        active = false;
    }

    public override void Enable()
    {
        active_tween?.Kill();
        active_tween = image.DOFade(1,0.2f).SetEase(Ease.OutQuad);
        active = true;
    }

    public override void Disable()
    {
        if(!active) return;
        active_tween?.Kill();
        active_tween = image.DOFade(0,0.2f).SetEase(Ease.OutQuad).OnComplete(()=> active = false);
    }

    public void MoveTo(Vector2 position)
    {
        move_tween?.Kill();
        if(!active)
        {
            transform.position = position;
        }
        else
        {
            move_tween = transform.DOMove(position,0.15f).SetEase(Ease.OutQuad);
        }
    }
}
