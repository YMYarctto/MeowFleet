using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ToPVEScene : BaseButton_Setting
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        SceneController.instance.AfterSceneLoadAction(()=>PVEController.instance.Init());
        SceneController.instance.ChangeScene(SceneRegistry.PVEScene);
    }
}
