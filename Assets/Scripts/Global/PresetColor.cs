using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PresetColor
{
    public static Color Button_Idle
    {
        get
        {
            Color color;
            ColorUtility.TryParseHtmlString("#000000", out color);
            return color;
        }
    }

    public static Color Button_Focus
    {
        get
        {
            Color color;
            ColorUtility.TryParseHtmlString("#B51600", out color);
            return color;
        }
    }
}
