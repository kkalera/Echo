using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AutoPilot
{
    public static float GetInputValue(float targetPosition, float spreaderPosition, float maxSpeed, float acceleration)
    {
        float distanceToTravel = Mathf.Abs(targetPosition - spreaderPosition);
        float input = distanceToTravel / (maxSpeed / acceleration);

        if (targetPosition < spreaderPosition) return Mathf.Clamp(-input, -1, 1);

        return Mathf.Clamp(input, -1, 1);
    }

    public static Vector3 GetInputs(Vector3 targetPosition, Vector3 spreaderPosition, float maxSpeed, float acceleration)
    {
        Vector3 inputs = new Vector3(0, 0, 0);

        // When target between legs
        if (targetPosition.z > -10 && targetPosition.z < 10)
        {
            float r = (spreaderPosition.y * 0.2f) + 1;

            // When going to container but not close enough to lower
            if (Mathf.Abs(spreaderPosition.z - targetPosition.z) > 1)
            {
                targetPosition = new Vector3(0, 16f, targetPosition.z);
            }
        }
        else
        {
            // When target is not between legs, but spreader is
            if (spreaderPosition.y < 15 && spreaderPosition.z > -10 && spreaderPosition.z < 10)
            {
                targetPosition = new Vector3(0, 20f, spreaderPosition.z);
            }

            if (spreaderPosition.y > 15 && spreaderPosition.z > -10 && spreaderPosition.z < 10)
            {
                targetPosition = new Vector3(0, 16f, targetPosition.z);
            }
        }



        float distanceToTravelY = Mathf.Abs(targetPosition.y - spreaderPosition.y);
        float inputY = distanceToTravelY / (maxSpeed / acceleration);

        float distanceToTravelZ = Mathf.Abs(targetPosition.z - spreaderPosition.z);
        float inputZ = distanceToTravelZ / (maxSpeed / acceleration);

        if (targetPosition.y < spreaderPosition.y) inputY = -inputY;
        if (targetPosition.z < spreaderPosition.z) inputZ = -inputZ;

        inputs.y = Mathf.Clamp(inputY, -1, 1);
        inputs.z = Mathf.Clamp(inputZ, -1, 1);

        return inputs;

    }
}
