using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public static void Start()
    {
        SeedController.instance.InitSeed();
        LoadOperation op = ResourceManager.instance.InitLoadDataManager();
        SceneController.instance.WaitWhenSceneLoadFinish(() => op.IsFinish);
        SceneController.instance.ChangeScene(SceneRegistry.FrontScene);
    }
}
