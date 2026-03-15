using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ChangePVEPage : BaseButton_Setting
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        if(!PVEController.instance.PlayerAction||PVEController.instance.OnAnim)
        {
            return;
        }
        BG_PVE.GetUIView().NextPage();
    }

}
