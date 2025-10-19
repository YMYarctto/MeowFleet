using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior
{
    public Vector2Int size;
    public List<Ships_Enum> target_ships;

    Status current_status;
    List<List<Vector2Int>> valid_layouts;

    enum Status
    {
        Hunt,
        Target,
    }

    enum HitResult
    {
        Miss,
        Hit,
        Destroyed,
    }
}
