using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class EnvGrabContainer : Environment
    {
        public override void InitializeEnvironment()
        {

        }
        public override void OnEpisodeBegin()
        {

        }
        public override State Step()
        {
            return new State();
        }
        public override void UpdateTargetWorldPosition(Vector3 position)
        {
            base.UpdateTargetWorldPosition(position);
        }
    }
}