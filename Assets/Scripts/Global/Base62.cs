using System;
using System.Text;

public static class Base62
{
    private const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Encode(int value)
    {
        if (value == 0)
            return chars[0].ToString();

        StringBuilder sb = new StringBuilder();
        bool negative = value < 0;
        value = Math.Abs(value);

        while (value > 0)
        {
            int index = value % 62;
            sb.Insert(0, chars[index]);
            value /= 62;
        }

        if (negative)
        sb.Append('-');

        return sb.ToString();
    }

    public static int Decode(string str)
    {
        int result = 0;

        bool negative = str[str.Length - 1] == '-';

        int end = negative ? 1 : 0;

        for (int i = 0; i < str.Length-end; i++)
        {
            int index = chars.IndexOf(str[i]);
            result = result * 62 + index;
        }

        return result * (negative ? -1 : 1);
    }
}

