using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObject : ScriptableObject
{

}
public class CustomRandom
{
    public int seed;
    public System.Random random;
    public CustomRandom(int seed)
    {
        random = new System.Random(seed);

    }
    public int Modifier(int[] modifiers)
    {
        foreach (int i in modifiers)
        {
            UnityEngine.Random.InitState(seed);
            seed = UnityEngine.Random.Range(Int32.MinValue, Int32.MaxValue)+i;
            random = new System.Random(seed);
        }
        return seed;
    }
}
