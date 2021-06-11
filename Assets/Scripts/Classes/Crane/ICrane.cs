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
    float MinSpreaderHeight { get; set; }
    bool SwingDisabled { set; get; }
    bool CraneMovementDisabled { set; get; }
    bool CabinMovementDisabled { set; get; }
    bool WinchMovementDisabled { set; get; }

    Transform Transform { get; }
    void ResetToRandomPosition();
    void ResetToPosition(Vector3 position);
    void SetWinchLimits(float minHeight, float maxHeight);
    void GrabContainer(Transform container);
    void ReleaseContainer(Transform newParent);

    Vector3 CranePosition { get; }
    Vector3 CraneWorldPosition { get; }
    Vector3 CraneVelocity { get; }

    Vector3 CabinPosition { get; }
    Vector3 CabinWorldPosition { get; }
    Vector3 CabinVelocity { get; }

    Vector3 SpreaderPosition { get; }
    Vector3 SpreaderWorldPosition { get; }
    Vector3 SpreaderVelocity { get; }
}
