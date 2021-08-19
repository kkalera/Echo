using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReporterTarget : MonoBehaviour
{
    public GameObject _receiver;
    private ICollisionReceiver receiver;

    private void Start()
    {
        receiver = _receiver.GetComponent<ICollisionReceiver>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        receiver.OnCollisionEnter(collision);

    }
    private void OnCollisionStay(Collision collision)
    {
        receiver.OnCollisionStay(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        receiver.OnCollisionExit(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        receiver.OnTriggerEnter(other);
    }
    private void OnTriggerStay(Collider other)
    {
        receiver.OnTriggerStay(other);
    }
    private void OnTriggerExit(Collider other)
    {
        receiver.OnTriggerExit(other);
    }
}
