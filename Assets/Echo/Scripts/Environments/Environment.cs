using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class Environment : MonoBehaviour
    {
        public virtual State Step() { return new State(); }
        public virtual void InitializeEnvironment() { }
        public virtual void OnEpisodeBegin() { }
        public virtual void UpdateTargetWorldPosition(Vector3 position)
        {
            TargetWorldPosition = position;
        }
        public Vector3 TargetWorldPosition { get; private set; }
        public virtual Vector3 CraneStartLocation () { return Vector3.zero; }
    }
}