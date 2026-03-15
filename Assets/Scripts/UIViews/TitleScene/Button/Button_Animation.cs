using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Animation : BaseButton_TitleScene
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        BGAnimator_TitleScene.GetUIView().ReEnable();
    }
}
