using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

namespace GP
{
    public class GrabAndPlaceAgent : Agent
    {
        [SerializeField] InputAction xMovementLeft;
        [SerializeField] InputAction xMovementRight;
        [SerializeField] InputAction zMovementForward;
        [SerializeField] InputAction zMovementBack;
        [SerializeField] Rigidbody agentBody;
        [SerializeField] Rigidbody objectBody;
        [SerializeField] float maxSpeed = 5f;
        [SerializeField] float acceleration = 5f;
        [SerializeField] string goalTag = "goal";

        private bool objectGrabbed;

        private void Start()
        {
            xMovementLeft.Enable();
            xMovementRight.Enable();
            zMovementForward.Enable();
            zMovementBack.Enable();
        }


        private static void AccelerateTo(Rigidbody body, Vector3 targetVelocity, float maxAccel, ForceMode forceMode = ForceMode.Acceleration)
        {
            Vector3 deltaV = targetVelocity - body.velocity;
            Vector3 accel = deltaV / Time.deltaTime;

            if (accel.sqrMagnitude > maxAccel * maxAccel)
                accel = accel.normalized * maxAccel;

            body.AddForce(accel, forceMode);
        }


        public override void OnEpisodeBegin()
        {
            objectGrabbed = false;
            transform.position = new Vector3(Random.Range(-4.5f, 4.5f), 0.1f, 0);
            transform.rotation = Quaternion.Euler(Vector3.zero);
            agentBody.velocity = Vector3.zero;
            agentBody.angularVelocity = Vector3.zero;

            objectBody.transform.position = new Vector3(Random.Range(-4.5f, 4.5f), 0.6f, 2);
            objectBody.velocity = Vector3.zero;
            objectBody.angularVelocity = Vector3.zero;

        }

        public override void CollectObservations(VectorSensor sensor)
        {
            AddReward(-1f / MaxStep);
            if (Vector3.Distance(transform.localPosition, objectBody.transform.localPosition) <= 1.1f) objectGrabbed = true;
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            Vector3 a = Vector3.zero;
            switch (actions.DiscreteActions[0])
            {
                case 1:
                    a.x = 1 * maxSpeed;
                    break;
                case 2:
                    a.x = -1 * maxSpeed;
                    break;
                case 3:
                    a.z = 1 * maxSpeed;
                    break;
                case 4:
                    a.z = -1 * maxSpeed;
                    break;
            }
            AccelerateTo(agentBody, a, acceleration);
            if(objectGrabbed) AccelerateTo(objectBody, a, acceleration);
        }
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> discrete = actionsOut.DiscreteActions;
            if (xMovementLeft.ReadValue<float>() > 0)
            {
                discrete[0] = 1;
            }
            else if (xMovementRight.ReadValue<float>() > 0)
            {
                discrete[0] = 2;
            }
            else if (zMovementBack.ReadValue<float>() > 0)
            {
                discrete[0] = 3;
            }
            else if (zMovementForward.ReadValue<float>() > 0)
            {
                discrete[0] = 4;
            }
            //Utils.ClearLogConsole();
            //Debug.Log(xMovementLeft.ReadValue<int>());
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(goalTag))
            {
                AddReward(1f);
                EndEpisode();
            }
        }
    }
}