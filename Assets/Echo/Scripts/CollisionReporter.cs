using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    [SerializeField] public GameObject CollisionReceiver;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("test");
        if (TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnCollisionEnter(collision);
        }
            
    }
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("test");
        if (TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnCollisionStay(collision);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("test");
        if (TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnCollisionExit(collision);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("test");
        if (TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnTriggerEnter(other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("test");
        if (TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnTriggerStay(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("test");
        if (TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnTriggerExit(other);
        }
    }
}
