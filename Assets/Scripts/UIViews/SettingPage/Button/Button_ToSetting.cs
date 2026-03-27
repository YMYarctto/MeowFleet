using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Button_ToSetting : BaseButton_Setting
{
    void OnEnable()
    {
        InputController.InputAction.System.ESC.started += ToSetting;
    }

    void OnDisable()
    {
        InputController.InputAction.System.ESC.started -= ToSetting;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Game.Setting();
    }

    void ToSetting(InputAction.CallbackContext ctx)
    {
        Game.Setting();
    }
}
