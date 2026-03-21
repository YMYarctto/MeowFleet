using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ToFormationPage : BaseButton_Setting
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        FormationController.instance.Show();
    }
}
