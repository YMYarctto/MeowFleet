using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Test : BaseButton_TitleScene
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        SceneController.instance.ChangeScene(SceneRegistry.TestScene);
    }
}
