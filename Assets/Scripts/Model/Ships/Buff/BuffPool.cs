using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffPool
{
    public bool Interferenced_core => SearchBuff(EBuff.Interferenced_core);
    public bool Interferenced_body => SearchBuff(EBuff.Interferenced_body);

    Dictionary<EBuff, Buff> _dict;

    public BuffPool()
    {
        _dict = new();
    }

    public void AddBuff(EBuff type, int round, int value=0)
    {
        if (_dict.ContainsKey(type))
        {
            _dict[type].Add(round, value);
        }
        else
        {
            _dict.Add(type, new Buff(type, round, value));
        }
    }

    public void NextRound()
    {
        List<EBuff> list = _dict.Keys.ToList();
        foreach(var key in list)
        {
            var buff = _dict[key];
            buff.NextRound();
            if(buff.RoundEnd)
            {
                _dict.Remove(key);
                Debug.Log($"{key}效果已结束");
            }
            else
            {
                _dict[key]=buff;
            }
        }
    }

    private bool SearchBuff(EBuff type)
    {
        if (_dict.ContainsKey(type))
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        string result = "{";
        foreach(var kv in _dict)
        {
            result += $"{kv.Key}:{kv.Value},";
        }
        result+="}";
        return result;
    }
}
