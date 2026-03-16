using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class Aim : UIView<Aim>
{
    Image image;

    Tween active_tween;
    Tween move_tween;
    bool active;

    RectTransform rect;
    Canvas canvas;

    public override void Init()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        canvas = GameObject.Find("PVEPage").GetComponent<Canvas>();
        image.color = new Color(1, 1, 1, 0);
        active = false;
    }

    public void MoveTo(Vector3 position)
    {
        move_tween?.Kill();

        if (!active)
        {
            transform.position = position;
        }
        else
        {
            move_tween = transform.DOMove(position,0.15f).SetEase(Ease.OutQuad);
        }
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

}
