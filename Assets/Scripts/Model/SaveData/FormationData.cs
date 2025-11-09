using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationData
{
    public Vector2Int size;
    public Dictionary<Vector2Int, int> dict;
    
    public FormationData(Vector2Int size, Dictionary<Vector2Int, int> dict)
    {
        this.size = size;
        this.dict = new(dict);
    }
}
