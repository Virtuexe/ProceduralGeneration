using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovingTargetScript : MonoBehaviour
{
    private float start;
    public float speed;
    public Vector3[] positions;
    private int currentPosition;
    private bool MovementRunning;
    IEnumerator coroutine;
    void Awake()
    {
        start = this.gameObject.transform.position.z;
    }
    public void Move(bool b)
    {
        if (!b)
            return;
        currentPosition++;
        if (currentPosition >= positions.Length)
            currentPosition = 0;
        if (MovementRunning)
        {
            StopCoroutine(coroutine);
        }
        coroutine = Movement();
        Debug.Log(coroutine);
        StartCoroutine(coroutine);
    }
    IEnumerator Movement()
    {
        MovementRunning=true;
        bool b=true;
        var firstPosition = this.gameObject.transform.position;
        while (b)
        {
            var step = Time.deltaTime * speed;
            var position = this.gameObject.transform.position;
            position = Vector3.MoveTowards(position, positions[currentPosition], step);
            if (Vector3.Distance(position, positions[currentPosition]) < 0.001f)
            {
                Debug.Log("end");
                this.gameObject.transform.position = positions[currentPosition];
                b = false;
                yield return null;
                MovementRunning = false;
            }
            else
            {
                this.gameObject.transform.position = position;
                yield return null;
            }
                
            
        }
    }
}
