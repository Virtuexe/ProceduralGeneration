using Generation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsScript : MonoBehaviour
{
    public static Task mainTask;
    public void Awake()
    {
        mainTask.minFPS = 100f;
    }
    private void Update()
    {
        mainTask.Start();
    }
}
