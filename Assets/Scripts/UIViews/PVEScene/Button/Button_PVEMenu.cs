using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_PVEMenu : BaseButton_Setting
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        //TODO FIX
        SceneController.instance.AfterSceneLoadAction(() =>
        {
            UIManager.instance.EnableUIView<BGAnimator_TitleScene>();
        });
        SceneController.instance.ChangeScene(SceneRegistry.TitleScene);
    }
}
