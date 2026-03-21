using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneController : Manager<SceneController>
{
    SceneLoader sceneLoader;
    string currentSceneName;
    string targetSceneName;

    Func<bool> waitWhenSceneLoadFinishAction;

    public void Init()
    {
        sceneLoader = SceneLoader.GetUIView();
        currentSceneName = string.Empty;
    }

    public void ChangeScene(string sceneName)
    {
        sceneLoader.Enable();
        targetSceneName = sceneName;
    }

    public void StartChangeSceneCoroutine()
    {
        StartCoroutine(EChangeScene());
    }

    public void AfterSceneLoadAction(UnityAction action)
    {
        sceneLoader.ActionAfterLoad += action;
    }

    public void AfterSceneUnloadAction(UnityAction action)
    {
        sceneLoader.ActionAfterUnload += action;
    }

    public void WaitWhenSceneLoadFinish(Func<bool> action)
    {
        waitWhenSceneLoadFinishAction = action;
    }

    IEnumerator EChangeScene()
    {
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentSceneName);

            while (!unloadOp.isDone)
            {
                yield return null;
            }

            System.GC.Collect();
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }
            yield return null;
        }
        currentSceneName = targetSceneName;
        yield return new WaitForSecondsRealtime(0.5f);
        if(waitWhenSceneLoadFinishAction != null)
        {
            yield return new WaitUntil(waitWhenSceneLoadFinishAction);
            waitWhenSceneLoadFinishAction = null;
        }
        sceneLoader.Disable();
    }
}
