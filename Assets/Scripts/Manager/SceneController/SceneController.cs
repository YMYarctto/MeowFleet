using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // LoadingScene loadingScene;
    string currentSceneName;
    string targetSceneName;

    private static SceneController _instance;
    public static SceneController instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<SceneController>();
                if (!_instance)
                {
                    return null;
                }
            }
            return _instance;
        }
    }

    public void Init()
    {
        // loadingScene = UIManager.instance.GetUIView<LoadingScene>();
        currentSceneName = string.Empty;
    }

    public void ChangeScene(string sceneName)
    {
        // UIManager.instance.EnableUIView<InteractionBarrier>();
        // loadingScene.Enable();
        targetSceneName = sceneName;
    }

    public void StartChangeSceneCoroutine()
    {
        StartCoroutine(EChangeScene());
    }

    public void OnSceneLoadAction(UnityAction action)
    {
        // loadingScene.ActionAfterLoad = action;
    }

    public void OnSceneUnloadAction(UnityAction action)
    {
        // loadingScene.ActionAfterUnload = action;
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
        // loadingScene.Disable();
    }
}
