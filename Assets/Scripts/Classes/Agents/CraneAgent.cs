using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class CraneAgent : Agent
{
    [Header("Variables")]
    [Space(10)]
    [SerializeField] private Environment environment;
    [SerializeField] private GameObject cranePrefab;
    [SerializeField] [Range(.1f, 10f)] private float craneSpeed = 1;
    [SerializeField] [Range(.1f, 10f)] private float katSpeed = 4;
    [SerializeField] [Range(1f, 100f)] private float liftSpeed = 100;


    [Header("Inputs")]
    [Space(10)]
    [SerializeField] private InputAction inputUp;
    [SerializeField] private InputAction inputDown;
    [SerializeField] private InputAction inputLeft;
    [SerializeField] private InputAction inputRight;
    [SerializeField] private InputAction inputLift;

    private Crane crane;
    private GameObject craneGameObject;

    private void Awake()
    {
        crane = GetComponentInChildren<Crane>();
    }

    private void Start()
    {
        inputUp.Enable();
        inputDown.Enable();
        inputLeft.Enable();
        inputRight.Enable();
        inputLift.Enable();
    }

    private void FixedUpdate()
    {
        if (crane != null && !environment.GetComponent<BoxCollider>().bounds.Contains(crane.Kat.transform.localPosition))
        {
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        // Destroy the current crane
        if (crane != null) Destroy(crane.gameObject);

        // Instatiate a new crane
        craneGameObject = Instantiate(cranePrefab, transform);
        crane = craneGameObject.GetComponent<Crane>();
        crane.Init();

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = inputUp.ReadValue<float>();
        continuousActions[1] = inputDown.ReadValue<float>();
        continuousActions[2] = inputLeft.ReadValue<float>();
        continuousActions[3] = inputRight.ReadValue<float>();
        continuousActions[4] = inputLift.ReadValue<float>();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<float> continousActions = actions.ContinuousActions;
        Vector3 movementVector = new Vector3((continousActions[3] - continousActions[2]) * katSpeed, 0, (continousActions[0] - continousActions[1]) * craneSpeed);
        crane.Kat.MoveTransform(movementVector * Time.deltaTime);

        crane.Hijs(continousActions[4] * liftSpeed);
    }
}
