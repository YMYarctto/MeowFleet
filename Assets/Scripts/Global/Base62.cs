using System;
using System.Text;

public static class Base62
{
    private const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Encode(long value)
    {
        if (value == 0)
            return chars[0].ToString();

        StringBuilder sb = new StringBuilder();

        while (value > 0)
        {
            int index = (int)(value % 62);
            sb.Insert(0, chars[index]);
            value /= 62;
        }

        return sb.ToString();
    }

    public static long Decode(string str)
    {
        long result = 0;

        for (int i = 0; i < str.Length; i++)
        {
            int index = chars.IndexOf(str[i]);
            result = result * 62 + index;
        }

        return result;
    }
}

