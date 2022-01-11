using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CollisionReceiver 
{
    public void OnCollisionEnter(Collision collision);
    public void OnCollisionStay(Collision collision);
    public void OnCollisionExit(Collision collision);
    public void OnTriggerEnter(Collider other);
    public void OnTriggerStay(Collider other);
    public void OnTriggerExit(Collider other);
}
