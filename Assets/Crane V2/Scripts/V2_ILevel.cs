using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo
{
    public interface V2_ILevel
    {
        public bool Dead { get; }
        public Vector3 TargetPosition(Vector3 environmentPosition);
        public void InitializeEnvironment(Transform environment, V2_Crane crane);
        public void ResetEnvironment();
        public V2_CollisionResponse OnCollision(Collision col);
    }
}