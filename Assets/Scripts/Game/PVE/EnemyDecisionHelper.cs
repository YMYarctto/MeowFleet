using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemyDecisionHelper
{
    public static List<Skill> OrderSkills(IEnumerable<Skill> skills)
    {
        return skills.OrderBy(skill => skill.Order).ToList();
    }

    public static int SelectTorpedoRow(
        Queue<Vector2Int> preferredCoords,
        List<int> excludedRows,
        Func<Vector2Int, bool> isPreferredCoordAvailable,
        Func<List<int>, int> selectCurrentMapRow,
        Func<List<int>, int> selectHuntMapRow)
    {
        while (preferredCoords.Count > 0)
        {
            Vector2Int coord = preferredCoords.Dequeue();
            if (excludedRows.Contains(coord.x))
            {
                continue;
            }

            if (isPreferredCoordAvailable(coord))
            {
                return coord.x;
            }
        }

        int row = selectCurrentMapRow(excludedRows);
        if (row >= 0)
        {
            return row;
        }

        return selectHuntMapRow(excludedRows);
    }
}
