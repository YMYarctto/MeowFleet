using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ToSetting : BaseButton_Setting
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        Game.Setting();
    }
}
