using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game
{
    public static void Start()
    {
        SeedController.instance.InitSeed();
        LoadOperation op = ResourceManager.instance.InitLoadDataManager();
        SceneController.instance.WaitWhenSceneLoadFinish(() => op.IsFinish);
        SceneController.instance.ChangeScene(SceneRegistry.FrontScene);
    }

    public static void ToPVE()
    {
        SceneController.instance.AfterSceneLoadAction(()=>PVEController.instance.Init());
        SceneController.instance.ChangeScene(SceneRegistry.PVEScene);
    }

    public static void Setting()
    {
        BGAnimator_GameSetting.GetUIView().SettingEnable();
        InputController.instance.LoadBindings();
        AudioManager.instance.LoadBindings();
    }

    public static void SettingReturn()
    {
        BGAnimator_GameSetting.GetUIView().SettingDisable();
        InputController.instance.SaveBindings();
        AudioManager.instance.SaveBindings();
    }

    public static void ToTitle()
    {
        Time.timeScale = 1f;
        SceneController.instance.AfterSceneLoadAction(() =>
        {
            BGAnimator_TitleScene.GetUIView().Enable();
        });
        SceneController.instance.ChangeScene(SceneRegistry.TitleScene);
        InputController.instance.SaveBindings();
        AudioManager.instance.SaveBindings();
    }
}
