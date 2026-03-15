using System;
using UnityEngine;

public class SeedController : MonoBehaviour
{
    public int Seed => seed;
    private int seed;

    private System.Random random;

    private static SeedController _instance;
    public static SeedController instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(SeedController)) as SeedController;
                if (!_instance)
                    return null;
            }
            return _instance;
        }
    }

    void Awake()
    {
        random = new System.Random();
    }

    public void InitSeed()
    {
        long ticks = DateTime.Now.Ticks;

        seed = (int)(ticks ^ (ticks >> 32));
        ApplySeed(seed);
    }

    public void SetSeed(int newSeed)
    {
        seed = newSeed;
        ApplySeed(seed);
    }

    void ApplySeed(int s)
    {
        random = new System.Random(s);
        Debug.Log($"Seed: {ToString()}");
    }

    public int Range(int min, int max)
    {
        return random.Next(min, max);
    }

    public float Range(float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }

    public override string ToString()
    {
        return Base62.Encode(seed);
    }
}
