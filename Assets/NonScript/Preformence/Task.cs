using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public struct Task
{
    public float minFPS;
    private float startTime;
    public void Start()
    {
        startTime = Time.realtimeSinceStartup;
	}
    public bool OutOfTime()
    {
		return 1/minFPS < Time.realtimeSinceStartup - startTime;
    }
}
