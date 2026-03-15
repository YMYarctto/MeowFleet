using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Setting : BaseButton_TitleScene
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        BGAnimator_TitleScene.GetUIView().SettingEnable();
        InputController.instance.LoadBindings();
        AudioManager.instance.LoadBindings();
    }
}
