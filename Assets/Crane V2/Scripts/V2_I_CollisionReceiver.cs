using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace echo
{
    public interface V2_I_CollisionReceiver
    {
        void OnCollision(Collision col);
        //V2_CollisionResponse OnCollision(Collision col);
    }
}
