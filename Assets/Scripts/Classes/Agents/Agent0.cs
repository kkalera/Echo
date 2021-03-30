using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Agent0 : Agent, IAgent
{
    private IEnvironment _Environment;
    public IEnvironment Environment { get => _Environment; set => _Environment = value; }

    public override void Initialize()
    {
        base.Initialize();

    }
}
