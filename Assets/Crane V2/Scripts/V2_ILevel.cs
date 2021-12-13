using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo
{
    public interface V2_ILevel
    {        
        public Vector3 TargetPosition(Vector3 environmentPosition);
        public void InitializeEnvironment(Transform environment, V2_Crane crane);
        public void ResetEnvironment();
    }
}