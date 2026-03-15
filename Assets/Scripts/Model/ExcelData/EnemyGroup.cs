using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyGroup
{
    public string uid;
    public int layer;
    public int size;
    public string enemy_list_string;
    public int weight;

    public List<int> enemy_list;
}