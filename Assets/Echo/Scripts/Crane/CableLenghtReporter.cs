using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Filo.Cable))]
public class CableLenghtReporter : MonoBehaviour
{
    [SerializeField] Echo.SoCraneSpecs craneSpecs;
    private Filo.Cable cable;

    private void Start()
    {
        cable = GetComponent<Filo.Cable>();
    }
    private void Update()
    {
        var joint = cable.links[0];
        craneSpecs.winchCableAmount = joint.storedCable;
    }
}
