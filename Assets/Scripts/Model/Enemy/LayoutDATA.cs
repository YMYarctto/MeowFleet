using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class LayoutDATA
{
    List<Vector2Int> _layout;
    List<Vector2Int> _layout_r1
    {
        get => _layout.ConvertAll(layout => new Vector2Int(layout.y, -layout.x));
    }
    List<Vector2Int> _layout_r2
    {
        get => _layout.ConvertAll(layout => new Vector2Int(-layout.x, -layout.y));
    }
    List<Vector2Int> _layout_r3
    {
        get => _layout.ConvertAll(layout => new Vector2Int(-layout.y, layout.x));
    }
    public List<Vector2Int> Current
    {
        get => _layout;
    }

    public LayoutDATA()
    {
        _layout = new();
    }

    public LayoutDATA(List<Vector2Int> layout)
    {
        _layout = new(layout);
    }

    public List<LayoutDATA> AllLayout()
    {
        return new(){new(_layout),new(_layout_r1),new(_layout_r2),new(_layout_r3)};
    }

    public List<Vector2Int> LayoutInMap(Vector2Int center)
    {
        return _layout.ConvertAll(v => center + v);
    }

    public List<Vector2Int> GetAdjacentCellsInMap(Vector2Int center)
    {
        HashSet<Vector2Int> result = new();
        HashSet<Vector2Int> original = new(LayoutInMap(center));

        Vector2Int[] directions =
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };

        foreach (var c in LayoutInMap(center))
        {
            foreach (var dir in directions)
            {
                Vector2Int neighbor = c + dir;
                // 只添加不在原始坐标中的点
                if (!original.Contains(neighbor))
                    result.Add(neighbor);
            }
        }

        return result.ToList();
    }

    public bool Contains(Vector2Int coord)
    {
        return _layout.Contains(coord);
    }

    public override string ToString()
    {
        string str = "[";
        foreach (var coord in _layout)
        {
            str += $" ({coord.x},{coord.y}) ";
        }
        str += "]";
        return str;
    }
}
