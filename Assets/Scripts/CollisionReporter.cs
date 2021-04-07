using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    public CraneAgent agent;

    private void OnCollisionEnter(Collision collision)
    {
        if (agent != null) agent.OnCollisionEnter(collision);

    }
    private void OnCollisionStay(Collision collision)
    {
        if (agent != null) agent.OnCollisionStay(collision);
    }
}
