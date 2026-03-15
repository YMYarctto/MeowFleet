using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_PVEMenu : BaseButton_Setting
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        BGAnimator_PVEScene.GetUIView().SettingEnable();
        InputController.instance.LoadBindings();
        AudioManager.instance.LoadBindings();
    }
}
