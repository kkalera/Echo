using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    /// <summary>
    /// Moves a rigidbody towards a position smoothly
    /// </summary>
    /// <param name="targetPosition">Target position</param>
    /// <param name="rb">The rigidbody to be moved</param>
    /// <param name="targetVel">The velocity to target during the motion</param>
    /// <param name="maxVel">The maximum velocity possible</param>
    public void MoveTowards(Vector3 targetPosition, Rigidbody rb, float targetVel, float maxVel)
    {
        Vector3 moveToPosition = targetPosition - rb.transform.localPosition;
        Vector3 velocityTarget = targetVel * moveToPosition;
        rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxVel);
    }
}
