using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Echo
{
    [RequireComponent(typeof(Rigidbody))]
    public class Spreader : MonoBehaviour
    {
        [SerializeField] private SoCraneSpecs craneSpecs;
        void Start()
        {
            craneSpecs.spreaderWorldPosition = transform.position;
            craneSpecs.spreaderBody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            craneSpecs.spreaderWorldPosition = transform.position;
        }
    }
}