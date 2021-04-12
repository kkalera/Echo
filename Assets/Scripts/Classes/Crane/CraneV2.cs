using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneV2 : MonoBehaviour, ICrane
{
    [Space(20)]
    [Header("Crane state")]
    [SerializeField] private bool craneMovementEnabled = true;
    [SerializeField] private bool cabinMovementEnabled = true;
    [SerializeField] private bool winchMovementEnabled = true;

    [Space(20)]
    [Header("Crane parts")]
    [SerializeField] private Transform cabin;
    [SerializeField] private Transform spreader;
    [SerializeField] private HingeJoint spoolLandRight;
    [SerializeField] private HingeJoint spoolLandLeft;
    [SerializeField] private HingeJoint spoolWaterRight;
    [SerializeField] private HingeJoint spoolWaterLeft;

    [Space(20)]
    [Header("Crane specs")]
    [SerializeField] [Range(.1f, 10f)] private float craneSpeed = 0.75f; //Speed in m/s
    [SerializeField] [Range(.1f, 1f)] private float craneAcceleration = 0.25f; //Acceleration in %
    [SerializeField] [Range(.1f, 10f)] private float cabinSpeed = 4; //Speed in m/s
    [SerializeField] [Range(.1f, 1f)] private float cabinAcceleration = 0.25f; //Acceleration in %

    private MovementManager movementManager;
    private Rigidbody cabinBody;
    private Rigidbody craneBody;
    private Rigidbody spreaderBody;

    public Vector3 CabinPosition => cabin.localPosition;

    public Vector3 CranePosition => transform.localPosition;

    public Vector3 SpreaderPosition => spreader.localPosition;

    private void Start()
    {
        movementManager = GetComponent<MovementManager>();
        cabinBody = cabin.GetComponent<Rigidbody>();
        spreaderBody = cabin.GetComponent<Rigidbody>();
        craneBody = this.GetComponent<Rigidbody>();
    }

    public void MoveCabin(float val)
    {
        if (!cabinMovementEnabled) return;
        if (cabin.localPosition.z > 45 && val > 0) val = 0;
        if (cabin.localPosition.z < -20 && val < 0) val = 0;
        float targetSpeed = movementManager.GetNextSpeed(val, cabinBody.velocity.z, cabinSpeed * cabinAcceleration * Time.deltaTime, cabinSpeed);
        Vector3 newVelocity = cabinBody.velocity;
        newVelocity.z = targetSpeed;
        cabinBody.velocity = newVelocity;
    }

    public void MoveCrane(float val)
    {
        if (!craneMovementEnabled) return;
        float targetSpeed = movementManager.GetNextSpeed(val, craneBody.velocity.x, craneSpeed * craneAcceleration * Time.deltaTime, craneSpeed);
        Vector3 newVelocity = craneBody.velocity;
        newVelocity.x = targetSpeed;
        craneBody.velocity = newVelocity;
    }

    public void MoveWinch(float value)
    {
        if (!winchMovementEnabled) return;
        if (value > 0 && spreader.localPosition.y > cabin.localPosition.y - 5) value = 0;

        // Adjust the value since the value provided is the speed in m/s
        // The motor target velocity is in degree/s
        // Since our pulleys have a diameter of 1m we want 1 rotation/pi per m/s requested.
        value *= 360 / Mathf.PI;

        // Get 2 motors (one side turns clockwise while the other side turns counter-clockwise
        JointMotor motor = spoolWaterLeft.motor;
        JointMotor motor2 = spoolLandLeft.motor;

        float currentVelocity = motor.targetVelocity;
        float acceleration = 360 * Time.deltaTime;

        if (value > currentVelocity && value != 0)
        {
            value = Mathf.Min(currentVelocity + acceleration, value);
        }
        else if (value < currentVelocity && value != 0)
        {
            value = Mathf.Max(currentVelocity - acceleration, value);
        }

        if (value == 0 && currentVelocity > 0)
        {
            value = Mathf.Max(currentVelocity - acceleration, value);
        }
        else if (value == 0 && currentVelocity < 0)
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
        cabin.localPosition = new Vector3(0, 32, Random.Range(-20, 45));
        spreader.localPosition = new Vector3(cabin.localPosition.x, 25, cabin.localPosition.z + 1f);
        Filo.Cable[] cables = GetComponentsInChildren<Filo.Cable>();
        cables[0].Setup();
        cables[1].Setup();
        cables[2].Setup();
        cables[3].Setup();


        spreaderBody.isKinematic = true;
        new WaitForSeconds(0.001f);
        spreaderBody.isKinematic = false;
    }
}
