using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ToPVEScene : BaseButton_Setting
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        if(FormationController.instance.EmptyMap)
        {
            Pop_up.GetUIView().Show("未放置舰船");
            return;
        }
        Game.ToPVE();
    }
}
