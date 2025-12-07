using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_TestReturnTitle : BaseButton_TitleScene
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        SceneController.instance.AfterSceneLoadAction(() =>
        {
            UIManager.instance.EnableUIView<BGAnimator_TitleScene>();
        });
        SceneController.instance.ChangeScene(SceneRegistry.TitleScene);
    }

}
