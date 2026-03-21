using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PVELoadData_SO", menuName = "Data/Load/PVELoadData_SO")]
public class PVELoadData_SO : ScriptableObject
{
    public List<int> EnemyGroupList;
    public int EnemyMapSize;

    public void SetLoadData(EnemyGroup enemyGroup)
    {
        EnemyGroupList = enemyGroup.enemy_list;
        EnemyMapSize = enemyGroup.size;
    }

    public void SetLoadData(List<int> enemyGroupList,int size)
    {
        EnemyGroupList = enemyGroupList;
        EnemyMapSize = size;
    }
}
