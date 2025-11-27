using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbabilityMapDATA
{
    Dictionary<Vector2Int, int> _map;

    public ProbabilityMapDATA()
    {
        _map = new();
    }

    public void AddProbability(Vector2Int coord)
    {
        if (_map.ContainsKey(coord))
        {
            _map[coord]++;
        }
        else
        {
            _map[coord] = 1;
        }
    }

    public void AddProbability(List<Vector2Int> coords)
    {
        foreach (var coord in coords)
        {
            AddProbability(coord);
        }
    }

    public void DeleteProbability(List<Vector2Int> coords)
    {
        foreach (var coord in coords)
        {
            if (_map.ContainsKey(coord))
            {
                _map[coord]--;
            }
        }
    }

    public void RemoveProbability(Vector2Int coord)
    {
        _map.Remove(coord);
    }

    public List<KeyValuePair<Vector2Int, int>> ToList()
    {
        return new List<KeyValuePair<Vector2Int, int>>(_map);
    }

    public override string ToString()
    {
        string str = "ProbabilityMapDATA:[\n";
        foreach (var kv in _map)
        {
            str += $"({kv.Key.x},{kv.Key.y}) : {kv.Value}\n";
        }
        str += "]\n";
        return str;
    }
}
