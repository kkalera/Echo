using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKat
{
    Transform kat { get; }

    void MoveTransform(Vector3 vector);

    void MoveRigidBody(Vector3 vector);
}
