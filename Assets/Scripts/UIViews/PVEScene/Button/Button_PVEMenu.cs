using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_PVEMenu : BaseButton_Setting
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<BGAnimator_PVEScene>().SettingEnable();
        InputController.instance.LoadBindings();
    }
}
