using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_StartGame : BaseButton_TitleScene
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        Game.Start();
    }
}
