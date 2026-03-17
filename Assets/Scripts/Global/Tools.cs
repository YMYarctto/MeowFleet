using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tools
{
    public static void Shuffle<T>(List<T> list,System.Random ran)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = ran.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
