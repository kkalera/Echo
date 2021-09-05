using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour, ICrane, ICollisionReceiver
{
    [Space(20)]
    [Header("Crane state")]
    [SerializeField] private bool craneMovementEnabled = true;
    [SerializeField] private bool cabinMovementEnabled = true;
    [SerializeField] private bool winchMovementEnabled = true;    
    [SerializeField] [Range(0.1f, 10f)] private float maxSwingDistance = 1;

    [Space(20)]
    [Header("Crane parts")]
    [SerializeField] private Transform cabin;
    [SerializeField] private Transform spreader;
    [SerializeField] private HingeJoint spoolLandRight;
    [SerializeField] private HingeJoint spoolLandLeft;
    [SerializeField] private HingeJoint spoolWaterRight;
    [SerializeField] private HingeJoint spoolWaterLeft;

    [SerializeField] private Filo.Cable cableLandRight;
    [SerializeField] private Filo.Cable cableLandLeft;
    [SerializeField] private Filo.Cable cableWaterRight;
    [SerializeField] private Filo.Cable cableWaterLeft;

    [Space(20)]
    [Header("Crane specs")]
    [SerializeField] [Range(.1f, 10f)] private float craneSpeed = 0.75f; //Speed in m/s
    [SerializeField] [Range(.1f, 1f)] private float craneAcceleration = 1f; //Acceleration in m/s?
    [SerializeField] [Range(.1f, 10f)] private float cabinSpeed = 4; //Speed in m/s
    [SerializeField] [Range(.1f, 1f)] private float cabinAcceleration = 1f; //Acceleration in m/s?
    [SerializeField] [Range(.1f, 10f)] private float winchSpeed = 4; //Speed in m/s    
    [SerializeField] private float minSpreaderHeight = 0;
    [SerializeField] private float maxSpreaderHeight = 0;
    
    private Rigidbody cabinBody;
    private Rigidbody craneBody;
    private Rigidbody spreaderBody;
    private Transform currentContainer;

    public Vector3 CabinPosition => cabin.position - transform.parent.position;
    public Vector3 CabinWorldPosition => cabin.position;
    public Vector3 CranePosition => transform.position - transform.parent.position;
    public Vector3 CraneWorldPosition => transform.position;
    public Vector3 SpreaderPosition => spreader.position - transform.parent.position;
    public Vector3 SpreaderWorldPosition => spreader.position;
    public Vector3 CraneVelocity => craneBody.velocity;
    public Vector3 CabinVelocity => cabinBody.velocity;
    public Vector3 SpreaderVelocity => spreaderBody.velocity;
    public Vector3 SpreaderRotation => spreaderBody.rotation.eulerAngles;
    public float MinSpreaderHeight { get => minSpreaderHeight; set => minSpreaderHeight = value; }
    public float SwingLimit { get => maxSwingDistance; set => maxSwingDistance = value; }
    public bool CraneMovementDisabled { get => !craneMovementEnabled; set => craneMovementEnabled = !value; }
    public bool CabinMovementDisabled { get => !cabinMovementEnabled; set => cabinMovementEnabled = !value; }
    public bool WinchMovementDisabled { get => !winchMovementEnabled; set => winchMovementEnabled = !value; }
    public Transform Transform { get => transform; }
    public bool ContainerGrabbed { get => currentContainer != null; }
    public bool SpreaderGrounded { get => IsSpreaderGrounded(); }

    private static void AccelerateTo(Rigidbody body, Vector3 targetVelocity, float maxAccel, ForceMode forceMode = ForceMode.Acceleration)
    {
        Vector3 deltaV = targetVelocity - body.velocity;
        Vector3 accel = deltaV / Time.deltaTime;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        body.AddForce(accel, forceMode);
    }

    private void Start()
    {
        cabinBody = cabin.GetComponent<Rigidbody>();
        spreaderBody = spreader.GetComponent<Rigidbody>();
        craneBody = GetComponent<Rigidbody>();
        maxSpreaderHeight = cabin.localPosition.y - 5;
    }

    private void FixedUpdate()
    {
        if(Mathf.Abs((CabinPosition.z + 1) - SpreaderPosition.z) > maxSwingDistance)
        {
            float zDiff = (CabinPosition.z + 1) - SpreaderPosition.z;
            Vector3 vel = spreaderBody.velocity;
            vel.z = cabinBody.velocity.z + zDiff;
            AccelerateTo(spreaderBody, vel, 1000, ForceMode.Acceleration);
            /*
            float zDiff = CabinVelocity.z - SpreaderVelocity.z;
            Vector3 force = new Vector3(0, 0, zDiff / (Time.deltaTime / Time.timeScale));
            Vector3 accel = force.normalized * Time.timeScale;
            spreaderBody.AddForce(accel, ForceMode.Impulse);*/
        }
        
    }

    public void MoveCabin(float val)
    {
        if (!cabinMovementEnabled) return;
        if (cabin.localPosition.z > 45 && val > 0) val = 0;
        if (cabin.localPosition.z < -30 && val < 0) val = 0;
        AccelerateTo(cabinBody, new Vector3(0, 0, cabinSpeed * val), cabinAcceleration);
    }

    public void MoveCrane(float val)
    {
        if (!craneMovementEnabled) return;
        if (transform.localPosition.x > 25 && val > 0) val = 0;
        if (transform.localPosition.x < -25 && val < 0) val = 0;
        AccelerateTo(cabinBody, new Vector3(craneSpeed * val, 0, 0), craneAcceleration);
    }

    public void MoveWinch(float value)
    {
        value *= winchSpeed;

        if (!winchMovementEnabled) return;
        if (value > 0 && spreader.localPosition.y > maxSpreaderHeight ) value = 0;
        if (value < 0 && spreader.localPosition.y < minSpreaderHeight) value = 0;


        Filo.Cable.Link linkLandLeft = cableLandLeft.links[0];
        if (linkLandLeft.storedCable < 5 && value < 0) value = 0;        


        // Adjust the value since the value provided is the speed in m/s
        // The motor target velocity is in degree/s
        // Since our pulleys have a diameter of 1m we want 1 rotation/pi per m/s requested.
        if (value != 0) value *= 360 / Mathf.PI;

        // Get 2 motors (one side turns clockwise while the other side turns counter-clockwise
        JointMotor motor = spoolWaterLeft.motor;
        JointMotor motor2 = spoolLandLeft.motor;

        float currentVelocity = motor.targetVelocity;
        //float acceleration = 360 * Time.deltaTime * Time.timeScale;
        float acceleration = 360 * Time.deltaTime;

        if (value != 0 && value > currentVelocity)
        {
            value = Mathf.Min(currentVelocity + acceleration, value);
        }
        else if (value != 0 && value < currentVelocity)
        {
            value = Mathf.Max(currentVelocity - acceleration, value);
        }

        if (Mathf.Approximately(value,0) && currentVelocity > 0)
        {
            value = Mathf.Max(currentVelocity - acceleration, value);
        }
        else if (Mathf.Approximately(value, 0) && currentVelocity < 0)
        {
            value = Mathf.Min(currentVelocity + acceleration, value);
        }

        motor.targetVelocity = value;
        motor2.targetVelocity = -value;

        spoolLandLeft.motor = motor;
        spoolLandRight.motor = motor2;
        spoolWaterLeft.motor = motor;
        spoolWaterRight.motor = motor2;

    }

    public void ResetToRandomPosition()
    {
        //spreaderBody.isKinematic = true;
        cabin.localPosition = new Vector3(0, 32, Random.Range(-20, 45));
        spreader.localPosition = new Vector3(cabin.localPosition.x, 25, cabin.localPosition.z + 1f);


        spreaderBody.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        spreader.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        spreaderBody.velocity = Vector3.zero;
        spreaderBody.angularVelocity = Vector3.zero;
        cabinBody.velocity = Vector3.zero;

        cableLandLeft.Setup();
        cableLandRight.Setup();
        cableWaterLeft.Setup();
        cableWaterRight.Setup();
        spreaderBody.isKinematic = false;
    }

    public void ResetToPosition(Vector3 position)
    {
        transform.localPosition = new Vector3(position.x, 0, 0f);
        cabin.localPosition = new Vector3(0, 32, position.z);
        spreader.localPosition = new Vector3(cabin.localPosition.x, position.y, cabin.localPosition.z + 1);

        spreaderBody.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        spreader.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        spreaderBody.velocity = Vector3.zero;
        spreaderBody.angularVelocity = Vector3.zero;
        cabinBody.velocity = Vector3.zero;
        spreaderBody.isKinematic = true;

        JointMotor motor = spoolWaterLeft.motor;
        JointMotor motor2 = spoolLandLeft.motor;
        motor.targetVelocity = 0;
        motor2.targetVelocity = 0;
        spoolLandLeft.motor = motor;
        spoolLandRight.motor = motor2;
        spoolWaterLeft.motor = motor;
        spoolWaterRight.motor = motor2;

        cableLandLeft.Setup();
        cableLandRight.Setup();
        cableWaterLeft.Setup();
        cableWaterRight.Setup();


        Filo.Cable.Link linkLandLeft = cableLandLeft.links[0];
        linkLandLeft.orientation = true;
        linkLandLeft.storedCable = 100;
        cableLandLeft.links[0] = linkLandLeft;

        Filo.Cable.Link linkLandRight = cableLandRight.links[0];
        linkLandRight.orientation = true;
        linkLandRight.storedCable = 100;
        cableLandRight.links[0] = linkLandRight;

        Filo.Cable.Link linkWaterLeft = cableWaterLeft.links[0];
        linkWaterLeft.orientation = false;
        linkWaterLeft.storedCable = 100;
        cableWaterLeft.links[0] = linkWaterLeft;

        Filo.Cable.Link linkWaterRight = cableWaterRight.links[0];
        linkWaterRight.orientation = false;
        linkWaterRight.storedCable = 100;
        cableWaterRight.links[0] = linkWaterRight;

        spreaderBody.isKinematic = false;
    }

    public void SetWinchLimits(float minHeight, float maxHeight)
    {
        minSpreaderHeight = Mathf.Max(minHeight, 0);
        maxSpreaderHeight = Mathf.Min(maxHeight, cabin.localPosition.y - 5);
    }

    public void GrabContainer(Transform container)
    {
        // Set the parent of the container to be the spreader. Attaching it to the spreader.
        container.parent = spreader;

        // Set the rotation of the container to match the current rotation of the spreader;
        //container.rotation = Quaternion.Euler(new Vector3(spreader.rotation.z, spreader.rotation.y, spreader.rotation.x));
        container.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));


        // Set the position of the container to match the position of the spreader
        // This prevents issues with the center of mass when the positions aren'ts matched exactly
        container.localPosition = new Vector3(0, -2.85f, 0);
        //container.localPosition = Vector3.zero;

        // Save the container as the current container
        currentContainer = container;

    }

    public void ReleaseContainer(Transform newParent)
    {
        // Check wether a current container exists
        if (currentContainer != null)
        {
            // Set the parent of the container to its new parent
            currentContainer.parent = newParent;

            // Set the current container back to null
            currentContainer = null;
        }
    }

    private bool IsSpreaderGrounded()
    {
                Collider[] hitGroundColliders = new Collider[3];
                var o = spreader.gameObject;
                Physics.OverlapBoxNonAlloc(
                    o.transform.position + new Vector3(0, -0.05f, 0),
                    new Vector3(0.95f / 2f, 0.5f, 0.95f / 2f),
                    hitGroundColliders,
                    o.transform.rotation);
                var grounded = false;
                foreach (var col in hitGroundColliders)
                {
                    if (col != null && col.transform != transform &&
                        (col.CompareTag("placed-container") ||
                         col.CompareTag("ground")
                         ))
                    {
                        grounded = true; //then we're grounded
                        break;
                    }
                }
                return grounded;
            
    }
   
    public void OnCollisionEnter(Collision collision)
    {
    }
    public void OnCollisionStay(Collision collision)
    {
    }
    public void OnCollisionExit(Collision collision)
    {
    }
    public void OnTriggerEnter(Collider other)
    {
    }

    public void OnTriggerStay(Collider other)
    {
    }

    public void OnTriggerExit(Collider other)
    {
    }
}
