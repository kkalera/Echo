using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICrane
{

    /// <summary>
    /// Move the entire crane along its tracks smoothly
    /// </summary>
    /// <param name="targetPosition">Target position</param>
    /// <param name="rb">Rigidbody to move</param>
    /// <param name="targetVelocity">Velocity to target during movement</param>
    /// <param name="maxVelocity">Maximum velocity it can achieve</param>
    void MoveCrane(float val);
    void MoveCabin(float val);
    void MoveWinch(float value);
    void ResetToRandomPosition();
    Vector3 CabinPosition { get; }
    Vector3 CranePosition { get; }
    Vector3 SpreaderPosition { get; }
}
