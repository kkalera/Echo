using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo
{
    public class V2_Spreader : MonoBehaviour
    {
        public V2_I_CollisionReceiver CollisionReceiver { get => _collisionReceiver; set => _collisionReceiver = value; }
        private V2_I_CollisionReceiver _collisionReceiver;

        public Vector3 environmentPosition { set => _environmentPosition = value; }
        public Vector3 Position { get => _Position(); set => transform.position = value; }
        public Rigidbody Rbody => GetComponent<Rigidbody>();

        private Vector3 _environmentPosition;
        private Vector3 _Position ()
        {
            if(_environmentPosition == null)
            {
                environmentPosition = Vector3.zero;
            }            

            return transform.position - _environmentPosition;
        }
        private void OnCollisionEnter(Collision collision)
        {
            _collisionReceiver.OnCollision(collision);
        }
    }
}