using UnityEngine;
using UnityEngine.UI;

public class RuntimeErrorViewer : MonoBehaviour
{
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color = "black";

        if(type == LogType.Log||type == LogType.Warning)
        {
            return;
        }

        RuntimeErrorBoeard.GetUIView().ShowError($"<color={color}>{logString}\n{stackTrace}\n</color>\n");
    }

}
