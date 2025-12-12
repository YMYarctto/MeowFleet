using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ChangePVEPage : BaseButton_Setting
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(!PVEController.instance.PlayerAction||PVEController.instance.OnAnim)
        {
            return;
        }
        UIManager.instance.GetUIView<BG_PVE>().NextPage();
    }

}
