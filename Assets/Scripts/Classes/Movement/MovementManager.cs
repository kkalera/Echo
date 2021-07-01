using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    /// <summary>
    /// Function that calculates the next speed in an acceleration movement
    /// </summary>
    /// <param name="inputValue"> The input between -1 and 1 </param>
    /// <param name="currentSpeed"> The current speed </param>
    /// <param name="acceleration"> The amount of acceleration from max speed (value between 0 and 1) </param>
    /// <param name="maxSpeed"> The maximum speed </param>
    /// <returns></returns>
    public float GetNextSpeed(float inputValue, float currentSpeed, float acceleration, float maxSpeed)
    {
        float requestedSpeed = inputValue * maxSpeed;
        float speed = 0;

        if (requestedSpeed >= currentSpeed && inputValue != 0)
        {
            speed = Mathf.Min(currentSpeed + maxSpeed * (acceleration * Time.deltaTime ), maxSpeed);
        }

        else if (requestedSpeed <= currentSpeed && inputValue != 0)
        {
            speed = Mathf.Max(currentSpeed - maxSpeed * (acceleration * Time.deltaTime ), -maxSpeed);
        }

        if (requestedSpeed == 0 && currentSpeed > 0)
        {
            speed = Mathf.Max(currentSpeed - maxSpeed * (acceleration * Time.deltaTime ), 0);
        }
        else if (requestedSpeed == 0 && currentSpeed < 0)
        {
            speed = Mathf.Min(currentSpeed + maxSpeed* (acceleration * Time.deltaTime ), 0);
        }

        return speed;
    }
}
