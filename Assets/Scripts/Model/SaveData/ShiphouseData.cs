using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiphouseData
{
    public int max_id;
    public Dictionary<int, Ship> dict;

    public ShiphouseData(Dictionary<int, Ship> dict)
    {
        max_id = 10000;
        this.dict = new(dict);
    }

    public ShiphouseData(int max_id, Dictionary<int, Ship> dict)
    {
        this.max_id = max_id;
        this.dict = new(dict);
    }
}
