using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spreader : MonoBehaviour
{
    [HideInInspector] public CraneAgent agent;

    private void OnCollisionEnter(Collision collision)
    {
        if (agent != null) { agent.OnCollisionEnter(collision); }
        else { Debug.Log("Agent is null"); }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (agent != null) { agent.OnCollisionStay(collision); }
        else { Debug.Log("Agent is null"); }
    }
}
