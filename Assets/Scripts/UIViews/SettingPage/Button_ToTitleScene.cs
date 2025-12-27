using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_ToTitleScene : BaseButton_Setting
{
    public override UIView currentView => this;

    public override void OnPointerClick(PointerEventData eventData)
    {
        Time.timeScale = 1f;
        SceneController.instance.AfterSceneLoadAction(() =>
        {
            UIManager.instance.EnableUIView<BGAnimator_TitleScene>();
        });
        SceneController.instance.ChangeScene(SceneRegistry.TitleScene);
    }
}
