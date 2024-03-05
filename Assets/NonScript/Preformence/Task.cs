using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public struct Task
{
    public float minFPS;
    public bool forceComplete;
    private float startTime;
    public void Start()
    {
        startTime = Time.realtimeSinceStartup;
	}
    public bool OutOfTime()
    {
        if (forceComplete) {
            return false;
        }
		return 1/minFPS < Time.realtimeSinceStartup - startTime;
    }
}
