using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_bomb : FX
{
    void OnEnable()
    {
        DestroySelf_Delay(0.5f);
    }
}
