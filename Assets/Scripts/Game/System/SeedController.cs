using System;
using UnityEngine;

public class SeedController : Manager<SeedController>
{
    public long Seed => seed;
    private long seed;

    public System.Random Random => random;
    private System.Random random;

    void Awake()
    {
        random = new System.Random();
    }

    public void InitSeed()
    {
        seed = DateTime.Now.Ticks;

        int ran = (int)(seed ^ (seed >> 32));
        ApplySeed(ran);
    }

    public void SetSeed(long newSeed)
    {
        seed = newSeed;
        int ran = (int)(seed ^ (seed >> 32));
        ApplySeed(ran);
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
