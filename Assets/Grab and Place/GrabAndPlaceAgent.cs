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
        [SerializeField] GameObject goalObject;
        [SerializeField] float maxSpeed = 5f;
        [SerializeField] float acceleration = 5f;
        [SerializeField] string goalTag = "goal";
        [SerializeField] bool autopilot = false;

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

            objectBody.transform.localPosition = new Vector3(12, 0.6f, 7.5f);
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
            }
            switch (actions.DiscreteActions[1])
            {
                case 1:
                    a.z = 1 * maxSpeed;
                    break;
                case 2:
                    a.z = -1 * maxSpeed;
                    break;
            }
            
            AccelerateTo(agentBody, a, acceleration);
            if(objectGrabbed) AccelerateTo(objectBody, a, acceleration);
            GetInput();

        }
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> discrete = actionsOut.DiscreteActions;

            if (autopilot)
            {
                var inputs = GetInput();
                discrete[0] = (int)inputs.y;
                discrete[1] = (int)inputs.x;

            }
            else
            {
                if (zMovementBack.ReadValueAsObject() != null)
                {
                    discrete[0] = 1;
                }
                else if (zMovementForward.ReadValueAsObject() != null)
                {
                    discrete[0] = 2;
                }


                if (xMovementLeft.ReadValueAsObject() != null)
                {
                    discrete[1] = 2;
                }
                else if (xMovementRight.ReadValueAsObject() != null)
                {
                    discrete[1] = 1;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(goalTag) && objectGrabbed)
            {
                AddReward(1f);
                EndEpisode();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("container"))
            {
                objectGrabbed = true;
            }
        }

        private Vector2 GetInput()
        {

            if (!objectGrabbed)
            {
                // Agent left of wall
                if (transform.localPosition.x > -4 && transform.localPosition.z < -1)
                {
                    return new Vector2(1, 0);
                }
                else if (transform.localPosition.z < objectBody.transform.localPosition.z)
                {
                    return new Vector2(1, 0);
                }
                else if (transform.localPosition.z > objectBody.transform.localPosition.z)
                {
                    return new Vector2(-1, 0);
                }
                else if (transform.localPosition.x > objectBody.transform.localPosition.x)
                {
                    return new Vector2(0, -1);
                }
            }
            else 
            {
                if (transform.localPosition.x > -4 && transform.localPosition.z > -1)
                {
                    return new Vector2(1, 0);
                }
                else if(transform.localPosition.z > goalObject.transform.localPosition.z)
                {
                    return new Vector2(-1, 0);
                }
                else if(transform.localPosition.x < goalObject.transform.localPosition.x)
                {
                    return new Vector2(0, -1);
                }
            }

            return Vector2.zero;
        }
    }
}