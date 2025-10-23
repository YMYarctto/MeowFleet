using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBarrier : UIView
{
    public override UIView currentView => this;

    public override void Init()
    {
        Disable();
    }
}