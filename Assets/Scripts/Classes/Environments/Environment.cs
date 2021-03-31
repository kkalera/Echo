using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public Vector3 EnvironmentSize { get; private set; }

    private void Awake()
    {
        EnvironmentSize = GetComponent<BoxCollider>().bounds.size;
    }
}
