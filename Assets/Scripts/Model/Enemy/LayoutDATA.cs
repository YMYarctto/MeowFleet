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
    public List<Vector2Int> ToList
    {
        get => _layout;
    }

    public List<Vector2Int> CoreList
    {
        get => ToList.GetRange(0, _core_number);
    }

    public List<Vector2Int> BodyList
    {
        get => ToList.GetRange(_core_number,ToList.Count-_core_number);
    }

    int _core_number;
    public int CoreNumber => _core_number;

    public LayoutDATA()
    {
        _layout = new();
        _core_number = 1;
    }

    public LayoutDATA(LayoutDATA layout)
    {
        _layout = new(layout.ToList);
        _core_number = layout._core_number;
    }

    public LayoutDATA(List<Vector2Int> layout,int core_number)
    {
        _layout = new(layout);
        _core_number = core_number;
        if(_core_number>layout.Count)
        {
            Debug.LogError("LayoutDATA: 错误的核心数量");
        }
    }

    public List<LayoutDATA> AllLayout()
    {
        int num = _core_number;
        return new(){new(_layout,num),new(_layout_r1,num),new(_layout_r2,num),new(_layout_r3,num)};
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

    public LayoutDATA Rotate(int direction)
    {
        int index = direction;
        while(index<0)
        {
            index += 4;
        }
        return AllLayout()[index % 4];
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
