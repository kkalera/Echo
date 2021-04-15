using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    public GameObject Environment;
    private IAgent agent;
    private void Start()
    {
        agent = Environment.GetComponent<IAgent>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (agent != null) agent.OnCollisionEnter(collision);

    }
    private void OnCollisionStay(Collision collision)
    {
        if (agent != null) agent.OnCollisionStay(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (agent != null) agent.OnTriggerEnter(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (agent != null) agent.OnTriggerStay(other);
    }
}
