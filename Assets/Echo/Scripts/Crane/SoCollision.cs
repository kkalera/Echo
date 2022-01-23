using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [CreateAssetMenu(fileName = "Collision", menuName = "ScriptableObjects/Collision", order = 1)]
    public class SoCollision : ScriptableObject
    {
        public Collision collision;
        public bool collided;
        public Collider triggered_collider;
        public bool triggered;
        public void Reset()
        {
            collision = null;
            triggered_collider = null;
            collided = false;
            triggered = false;
        }
    }
}