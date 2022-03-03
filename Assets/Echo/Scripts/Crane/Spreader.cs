using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Echo
{
    [RequireComponent(typeof(Rigidbody))]
    public class Spreader : MonoBehaviour
    {
        [SerializeField] private SoCraneSpecs craneSpecs;
<<<<<<< Updated upstream
        [SerializeField] private SoCollision collisionManager;
=======
        [SerializeField] private Crane crane;
>>>>>>> Stashed changes

        public Vector3 Position { get => transform.position; }
        public Quaternion Rotation { get => transform.rotation; }
        public Rigidbody Rbody { get => GetComponent<Rigidbody>(); }
<<<<<<< Updated upstream
        
=======

>>>>>>> Stashed changes
        public void GrabContainer(Transform container)
        {
            if(container.TryGetComponent<Rigidbody>(out Rigidbody rb)){
                rb.isKinematic = true;
            }
            container.parent = transform;
<<<<<<< Updated upstream
            collisionManager.collided = false;
            collisionManager._collision = null;
        }
        private void OnCollisionEnter(Collision collision)
        {
            collisionManager.collided = true;
            collisionManager._collision = collision;
        }
=======
            container.transform.rotation = Quaternion.Euler(Vector3.zero);
            container.transform.position = new Vector3(transform.position.x, container.transform.position.y, transform.position.z);
>>>>>>> Stashed changes

        }
    }
}