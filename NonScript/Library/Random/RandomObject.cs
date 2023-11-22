using System;
using UnityEngine;

public class RandomObject : ScriptableObject
{

}
public struct CustomRandom
{
    public int seed;
    public System.Random random;
    public CustomRandom(int seed, int[] modifiers)
    {
        this.seed = seed;
        this.random = new System.Random(seed);
        foreach (int i in modifiers)
        {
            seed = random.Next(Int32.MinValue, Int32.MaxValue) + i;
            random = new System.Random(seed);
        }
    }
}
