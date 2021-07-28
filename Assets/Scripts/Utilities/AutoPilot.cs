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

    public static Vector3 GetInputs(Vector3 targetPosition, Vector3 spreaderPosition, Vector3 currentSpeed, float acceleration)
    {

        Vector3 inputs = new Vector3(0, 0, 0);

        bool behindLegs = spreaderPosition.z < -10.5f;
        bool betweenLegs = spreaderPosition.z > -10.5f && spreaderPosition.z < 10.5f;
        bool inFrontOfLegs = spreaderPosition.z > 10.5f;

        // Check if we're to far from the target to lower the spreader        
        bool hasToCrossLeg = spreaderPosition.z > 10.5f && targetPosition.z < 10.5f;
        if (!hasToCrossLeg) hasToCrossLeg = spreaderPosition.z > -10.5f && targetPosition.z < -10.5f;
        if (!hasToCrossLeg) hasToCrossLeg = ((spreaderPosition.z > -10.5f && spreaderPosition.z < 10.5f) && (targetPosition.z > 10.5f || targetPosition.z < -10.5f));

        float r = (spreaderPosition.y * 0.2f) + 1;
        if (spreaderPosition.y < 19 && Mathf.Abs(spreaderPosition.z - targetPosition.z) > r && hasToCrossLeg)
        {
            targetPosition = new Vector3(0, 25f, spreaderPosition.z);
        }
        else if (spreaderPosition.y >= 19 && Mathf.Abs(spreaderPosition.z - targetPosition.z) > r)
        {
            targetPosition = new Vector3(0, spreaderPosition.y, targetPosition.z);

        }


        if (spreaderPosition.y - targetPosition.y > 1 &&
            spreaderPosition.z > 4 &&
            Mathf.Abs(spreaderPosition.z - targetPosition.z) < r) targetPosition.z -= 0.25f;


        float distanceToTravelY = Mathf.Abs(targetPosition.y - spreaderPosition.y);        
        float inputY = distanceToTravelY / (Mathf.Max(Mathf.Abs(currentSpeed.y), 0.05f) / acceleration) ;

        
        
        float distanceToTravelZ = Mathf.Abs(targetPosition.z - spreaderPosition.z);        
        float inputZ = distanceToTravelZ / (Mathf.Max(Mathf.Abs(currentSpeed.z), 0.05f) / acceleration) ;


        if (targetPosition.y < spreaderPosition.y) inputY = -inputY;
        if (targetPosition.z < spreaderPosition.z) inputZ = -inputZ;

        if (float.IsNaN(inputY)) inputY = 0;
        if (float.IsNaN(inputZ)) inputZ = 0;

        inputs.y = Mathf.Clamp(inputY, -1, 1);
        inputs.z = Mathf.Clamp(inputZ, -1, 1);

        return inputs;

    }
    public static Vector3 GetInputsSwing(Vector3 targetPosition, Vector3 spreaderPosition, Vector3 cabinPosition, Vector3 currentSpreaderSpeed, Vector3 currentKatSpeed, float acceleration)
    {
        Vector3 inputs = new Vector3(0, 0, 0);

        
        bool hasToCrossLeg = spreaderPosition.z > 10.5f && targetPosition.z < 10.5f;
        if (!hasToCrossLeg) hasToCrossLeg = spreaderPosition.z > -10.5f && targetPosition.z < -10.5f;
        if (!hasToCrossLeg) hasToCrossLeg = ((spreaderPosition.z > -10.5f && spreaderPosition.z < 10.5f) && (targetPosition.z > 10.5f || targetPosition.z < -10.5f));


        // Check if we're to far from the target to lower the spreader        
        float r = (spreaderPosition.y * 0.2f) + 1;
        if (spreaderPosition.y < 19 && Mathf.Abs(spreaderPosition.z - targetPosition.z) > r && hasToCrossLeg)
        {
            targetPosition = new Vector3(0, 25f, spreaderPosition.z);
        }
        else if (spreaderPosition.y >= 19 && Mathf.Abs(spreaderPosition.z - targetPosition.z) > r)
        {
            targetPosition = new Vector3(0, spreaderPosition.y, targetPosition.z);
        }


        if (spreaderPosition.y - targetPosition.y > 1 && spreaderPosition.z > 4 && Mathf.Abs(spreaderPosition.z - targetPosition.z) < r) targetPosition.z -= 0.25f;


        float distanceToTravelY = Mathf.Abs(targetPosition.y - spreaderPosition.y);
        float inputY = distanceToTravelY / (Mathf.Max(Mathf.Abs(currentKatSpeed.y), 0.05f) / acceleration);



        float distanceToTravelZ = Mathf.Abs(targetPosition.z - spreaderPosition.z);
        float inputZ = distanceToTravelZ / (Mathf.Max(Mathf.Abs(currentKatSpeed.z), 0.05f) / acceleration);


        if (targetPosition.y < spreaderPosition.y) inputY = -inputY;
        if (targetPosition.z < spreaderPosition.z) inputZ = -inputZ;

        if (float.IsNaN(inputY)) inputY = 0;
        if (float.IsNaN(inputZ)) inputZ = 0;

        inputs.y = Mathf.Clamp(inputY, -1, 1);
        inputs.z = Mathf.Clamp(inputZ, -1, 1);

        return inputs;

    }

    public static Vector3 GetInputsSwing2(Vector3 targetPosition, Vector3 spreaderPosition, Vector3 cabinPosition, Vector3 currentSpreaderSpeed, Vector3 currentKatSpeed, float acceleration)
    {

        return Vector3.zero;
    }
    }
