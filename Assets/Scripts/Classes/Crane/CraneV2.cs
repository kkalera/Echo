using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneV2 : MonoBehaviour, ICrane
{
    [SerializeField] private Transform cabin;
    [SerializeField] private Transform spreader;
    [SerializeField] private HingeJoint spoolLandRight;
    [SerializeField] private HingeJoint spoolLandLeft;
    [SerializeField] private HingeJoint spoolWaterRight;
    [SerializeField] private HingeJoint spoolWaterLeft;

    [Space(20)]
    [Header("Crane specs")]
    [SerializeField] [Range(.1f, 10f)] private float craneSpeed = 0.75f; //Speed in m/s
    [SerializeField] [Range(.1f, 10f)] private float cabinSpeed = 4; //Speed in m/s

    private MovementManager movementManager;
    private Rigidbody cabinBody;
    private Rigidbody craneBody;
    private Rigidbody spreaderBody;



    private void Start()
    {
        movementManager = GetComponent<MovementManager>();
        cabinBody = cabin.GetComponent<Rigidbody>();
        spreaderBody = cabin.GetComponent<Rigidbody>();
        craneBody = this.GetComponent<Rigidbody>();
    }

    public void MoveCabin(float val)
    {
        Vector3 newPos = cabin.localPosition;
        if (val < 0)
        {
            newPos += Vector3.back;
        }
        else if (val > 0)
        {
            newPos += Vector3.forward;
        }
        movementManager.MoveTowards(newPos, cabinBody, cabinSpeed, cabinSpeed);
    }

    public void MoveCrane(float val)
    {
        Vector3 newPos = transform.localPosition;
        if (val < 0)
        {
            newPos += Vector3.left;
        }
        else if (val > 0)
        {
            newPos += Vector3.right;
        }
        movementManager.MoveTowards(newPos, craneBody, craneSpeed, craneSpeed);
    }

    public void MoveWinch(float value)
    {
        throw new System.NotImplementedException();
    }
}
