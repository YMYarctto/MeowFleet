using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ChangePVEPage : BaseButton_Default
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<BG_PVE>().NextPage();
    }

}
