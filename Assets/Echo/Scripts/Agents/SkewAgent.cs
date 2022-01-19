using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SkewAgent : Agent
{
    [SerializeField] SkewManager skewManager;
    [SerializeField] Transform spreader;
    [SerializeField] GameObject projectilePrefab;
    private GameObject projectile = null;

    public override void OnEpisodeBegin()
    {
        skewManager.transform.rotation = Quaternion.Euler(Vector3.zero);
        spreader.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var disActions = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.RightArrow)) disActions[2] = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) disActions[1] = 1;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions[0];
        
        switch (action)
        {
            case 1: 
                skewManager.Skew(-1);
                break;
            case 2:
                skewManager.Skew(1);
                break;
        }

        if (Random.value < 0.01f && projectile == null)
        {
            ShootRandomProjectile();
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {       
        var skewAngle = skewManager.transform.eulerAngles.y;
        if (skewAngle >= 350) skewAngle = -360 + skewAngle;

        var spreaderAngle = spreader.eulerAngles.y;
        if (spreaderAngle >= 300) spreaderAngle = -360 + spreaderAngle;

        sensor.AddObservation(skewAngle);
        sensor.AddObservation(spreaderAngle);

        var reward = -1 / MaxStep;
        if (Mathf.Abs(spreaderAngle) < 0.5f) reward = 1;
        AddReward(reward);
    }

    private void ShootRandomProjectile()
    {
        var rval = Random.value;
        var spawnLocation = Vector3.zero;
        var direction = Vector3.zero;
        if(rval < 0.25)
        {
            spawnLocation = new Vector3(-4.5f, -0.5f, -10f);
            direction = new Vector3(0, 0, 500);
        }
        else if(rval >= 0.25 && rval < 0.5)
        {
            spawnLocation = new Vector3(4.5f, -0.5f, -10f);
            direction = new Vector3(0, 0, 500);
        }
        else if(rval >= 0.5 && rval < 0.75)
        {
            spawnLocation = new Vector3(4.5f, -0.5f, 10f);
            direction = new Vector3(0, 0, -500);
        }
        else if(rval >= 0.75)
        {
            spawnLocation = new Vector3(-4.5f, -0.5f, 10f);
            direction = new Vector3(0, 0, -500);
        }

        projectile = Instantiate(projectilePrefab, spawnLocation, Quaternion.Euler(Vector3.zero), transform);
        var rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(direction, ForceMode.Acceleration);
        

    }
}
