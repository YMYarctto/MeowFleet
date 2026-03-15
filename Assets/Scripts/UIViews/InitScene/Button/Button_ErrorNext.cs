using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ErrorNext : BaseButton_Default
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        RuntimeErrorBoeard.GetUIView().Disable();
    }
}
