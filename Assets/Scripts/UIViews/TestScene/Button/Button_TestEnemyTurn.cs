using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_TestEnemyTurn : BaseButton_TitleScene
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        EventManager.instance.Invoke(EventRegistry.TestScene.EnemyTurn);
    }

}
