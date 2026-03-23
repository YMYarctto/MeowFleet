using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
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

    private static readonly Dictionary<int, string> romanMap = new Dictionary<int, string>
    {
        {1000, "M"}, {900, "CM"}, {500, "D"}, {400, "CD"},
        {100, "C"}, {90, "XC"}, {50, "L"}, {40, "XL"},
        {10, "X"}, {9, "IX"}, {5, "V"}, {4, "IV"},
        {1, "I"}
    };

    // 将整数转换为罗马数字
    public static string IntToRoman(int num)
    {
        string result = "";
        foreach (var pair in romanMap)
        {
            while (num >= pair.Key)
            {
                result += pair.Value;
                num -= pair.Key;
            }
        }
        Debug.Log(result);
        return result;
    }
}
