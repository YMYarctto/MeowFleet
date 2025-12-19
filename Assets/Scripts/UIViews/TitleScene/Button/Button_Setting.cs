using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Setting : BaseButton_TitleScene
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<BGAnimator_TitleScene>().SettingEnable();
        InputController.instance.LoadBindings();
    }
}
