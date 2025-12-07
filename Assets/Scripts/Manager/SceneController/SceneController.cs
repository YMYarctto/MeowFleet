using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    SceneLoader sceneLoader;
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
        sceneLoader = UIManager.instance.GetUIView<SceneLoader>();
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
        sceneLoader.ActionAfterLoad = action;
    }

    public void AfterSceneUnloadAction(UnityAction action)
    {
        sceneLoader.ActionAfterUnload = action;
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
        yield return new WaitForSeconds(0.5f);
        sceneLoader.Disable();
    }
}
