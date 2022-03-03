using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    [SerializeField] public GameObject _collisionReceiver;

    private void OnCollisionEnter(Collision collision)
    {
        if (_collisionReceiver.TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnCollisionEnter(collision);
        }
            
    }
    private void OnCollisionStay(Collision collision)
    {
        if (_collisionReceiver.TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnCollisionStay(collision);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (_collisionReceiver.TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnCollisionExit(collision);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_collisionReceiver.TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnTriggerEnter(other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (_collisionReceiver.TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnTriggerStay(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (_collisionReceiver.TryGetComponent<CollisionReceiver>(out CollisionReceiver receiver))
        {
            receiver.OnTriggerExit(other);
        }
    }
}
