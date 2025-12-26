using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_KeyReset : BaseButton_Setting
{
    public override UIView currentView => this;

    Tween tween;
    Vector3 target = new(1.1f,1.1f,1.1f);

    public override void OnPointerClick(PointerEventData eventData)
    {
        InputController.instance.ResetToDefault();
    }

    protected override void DoEnter()
    {
        tween?.Kill();
        tween = transform.DOScale(target,0.1f).SetEase(Ease.OutQuad);
    }

    protected override void DoExit()
    {
        tween?.Kill();
        tween = transform.DOScale(Vector3.one,0.1f).SetEase(Ease.InQuad);
    }
}
