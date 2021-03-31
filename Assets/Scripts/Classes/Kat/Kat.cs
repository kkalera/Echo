using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kat : MonoBehaviour
{
    public void MoveTransform(Vector3 vector)
    {
        transform.localPosition += vector;
    }
    public void MoveRigidbody(Vector3 vector)
    {
        throw new System.NotImplementedException();
    }
}
