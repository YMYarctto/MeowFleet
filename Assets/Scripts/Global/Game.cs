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
}
