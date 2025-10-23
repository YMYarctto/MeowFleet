using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutDATA
{
    List<Vector2Int> _layout;
    List<Vector2Int> _layout_mirror
    {
        get => _layout.ConvertAll(layout => new Vector2Int(layout.y, layout.x));
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

    public LayoutDATA Mirror()
    {
        return new LayoutDATA(_layout_mirror);
    }

}
