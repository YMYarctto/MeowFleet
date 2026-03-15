using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOperation
{
    public bool IsFinish => isFinish;
    bool isFinish;

    public LoadOperation()
    {
        isFinish = false;
    }

    public void Finish()
    {
        isFinish = true;
    }
}
