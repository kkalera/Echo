using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo
{
    public class V2_CollisionReporter: MonoBehaviour
    {
        [SerializeField] GameObject collisionHandler;

        private void OnCollisionEnter(Collision collision)
        {
            collisionHandler.GetComponent<V2_I_CollisionReceiver>().OnCollision(collision);
        }
    }
}
