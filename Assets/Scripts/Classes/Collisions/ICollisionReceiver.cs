using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionReceiver
{    void OnCollisionEnter(Collision collision);
    void OnCollisionStay(Collision collision);
    void OnCollisionExit(Collision collision);
    void OnTriggerEnter(Collider other);
    void OnTriggerStay(Collider other);
    void OnTriggerExit(Collider other);
}
