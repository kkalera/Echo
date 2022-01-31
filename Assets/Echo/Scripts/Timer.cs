using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer:MonoBehaviour
{
    private float startTime;
    public void StartTimer()
    {
        startTime = Time.time;
    }
    public void EndTimer()
    {
        Debug.Log("Timer ended after: " + (Time.time - startTime));
    }
}
