using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : UIView
{
    public override UIView currentView => this;

    public override void Init()
    {
        base.Disable();
    }
}
