using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hudBarScript : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    public float size;
    private float _size;
    public float maxSize;

    private void OnValidate()
    {
        this.gameObject.transform.localScale = new Vector2(size/100, this.gameObject.transform.localScale.y);
        this.gameObject.transform.localPosition = new Vector2((-maxSize+size*500/100)/2, this.gameObject.transform.localPosition.y);
    }
    public void ReverseScale(float value)
    {
        size = 100;
        size -= value;
        OnValidate();
    }
    public void Scale(float value)
    {
        size = value;
        OnValidate();
    }
}
