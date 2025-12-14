using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_bomb2 : FX
{
    void OnEnable()
    {
        DestroySelf_Delay(0.5f);
    }
}
