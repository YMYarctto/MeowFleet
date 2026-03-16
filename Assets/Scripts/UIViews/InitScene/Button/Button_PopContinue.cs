using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_PopContinue : BaseButton_Setting
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        Pop_up.GetUIView().Disable();
    }

}
