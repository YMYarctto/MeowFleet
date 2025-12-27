using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ReturnGame : BaseButton_Setting
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<BGAnimator_PVEScene>().SettingDisable();
    }
}
