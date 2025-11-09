using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveData_SO", menuName = "Data/SaveData_SO", order = 1)]
public class SaveData_SO : ScriptableObject
{
    SaveData saveData;

    public void New()
    {
        saveData = new SaveData();
        saveData.shiphouseData = new ShiphouseData(new Dictionary<int, Ship>());
        saveData.formationData = new FormationData(new Vector2Int(8, 8), new Dictionary<Vector2Int, int>());
    }

    public int GetShiphouseData(out Dictionary<int, Ship> dict)
    {
        dict = new(saveData.shiphouseData.dict);
        return saveData.shiphouseData.max_id;
    }

    public void SetShiphouseData(int max_id, Dictionary<int, Ship> dict)
    {
        saveData.shiphouseData = new ShiphouseData(max_id, dict);
    }

    public void GetFormationData(out Dictionary<Vector2Int, int> dict)
    {
        dict = new(saveData.formationData.dict);
    }

    public void SetFormationData(Vector2Int size,Dictionary<Vector2Int, int> dict)
    {
        saveData.formationData.size = size;
        saveData.formationData = new FormationData(saveData.formationData.size, dict);
    }
}
