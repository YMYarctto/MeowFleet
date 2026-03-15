using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ReturnGame : BaseButton_Setting
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        BGAnimator_PVEScene.GetUIView().SettingDisable();
        InputController.instance.SaveBindings();
        AudioManager.instance.SaveBindings();
    }
}
