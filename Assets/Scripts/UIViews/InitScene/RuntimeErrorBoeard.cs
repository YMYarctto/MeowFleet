using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuntimeErrorBoeard : UIView
{
    public override UIView currentView => this;

    TMP_Text text;

    public override void Init()
    {
        text = transform.Find("Text").GetComponent<TMP_Text>();
        Disable();
    }

    public void ShowError(string error_str)
    {
        text.text = error_str;
        Enable();
    }

    public void CopyError()
    {
        GUIUtility.systemCopyBuffer = text.text;
    }
}
