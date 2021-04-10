using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CraneAgentV2 : Agent
{
    [SerializeField] private GameObject craneObject;
    [Header("Inputs")]
    [Space(10)]
    [SerializeField] private InputAction inputCabin;
    [SerializeField] private InputAction inputCrane;
    [SerializeField] private InputAction inputLift;

    private ICrane crane;

    void Start()
    {
        inputCabin.Enable();
        inputCrane.Enable();
        inputLift.Enable();
        crane = craneObject.GetComponent<ICrane>();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = inputCrane.ReadValue<float>();
        continuousActions[1] = inputCabin.ReadValue<float>();
        continuousActions[2] = inputLift.ReadValue<float>();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<float> continousActions = actions.ContinuousActions;
        crane.MoveCrane(continousActions[0]);
        crane.MoveCabin(continousActions[1]);
    }
}
