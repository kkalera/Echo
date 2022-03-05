using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Echo
{
    [RequireComponent(typeof(Rigidbody))]
    public class Spreader : MonoBehaviour
    {
        public Vector3 Position { get => transform.position; }
        public Quaternion Rotation { get => transform.rotation; }
        public Rigidbody Rbody { get => GetComponent<Rigidbody>(); }

        public void GrabContainer(Transform container)
        {
            if(container.TryGetComponent<Rigidbody>(out Rigidbody rb)){
                rb.isKinematic = true;
            }
            container.parent = transform;
        }
    }
}