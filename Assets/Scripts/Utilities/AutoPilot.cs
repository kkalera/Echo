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
        targetPosition = GetNextPosition(spreaderPosition, targetPosition);

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
    public static Vector3 GetInputsSwing(Vector3 targetPosition, Vector3 spreaderPosition, Vector3 cabinPosition, Vector3 currentSpreaderSpeed, Vector3 currentKatSpeed,Vector3 spreaderAngularVelocity, float acceleration)
    {        
        Vector3 inputs = new Vector3(0, 0, 0);

        // Get the next position in case we can't travel straight towards the provided target
        // This is to prevent colliding with the crane
        targetPosition = GetNextPosition(spreaderPosition, targetPosition);

        // Y input calculations to control the winch
        // This might change after the swing control has been perfected,
        // since winch movement also inpacts swing movement
        float distanceToTravelY = Mathf.Abs(targetPosition.y - spreaderPosition.y);
        float inputY = distanceToTravelY / (Mathf.Max(Mathf.Abs(currentSpreaderSpeed.y), 0.05f) / (acceleration));


        // Z input calculations
        float angle = Vector3.SignedAngle(new Vector2(cabinPosition.z + 1, cabinPosition.y), new Vector2(spreaderPosition.z, cabinPosition.y), Vector3.up);
        float length = Vector3.Distance(spreaderPosition, cabinPosition + new Vector3(0,0,1));       
        float maxSpreaderVelocity = Mathf.Sqrt(2 * 9.81f * length * (1 - Mathf.Cos(angle)));
        //maxSpreaderVelocity /= 10;

        float distanceToTravelZSpreader = Mathf.Min(16, Mathf.Abs(targetPosition.z - spreaderPosition.z ));
        float distanceToTravelZKat = Mathf.Min(16 ,Mathf.Abs(targetPosition.z - (cabinPosition.z+1)));
        float dDiff = Mathf.Abs((cabinPosition.z - 1) - spreaderPosition.z);

        float inputZ = distanceToTravelZKat / 16;
        
        float speedZ = Mathf.Abs(currentKatSpeed.z);
        if (cabinPosition.z - 1 > spreaderPosition.z) speedZ += maxSpreaderVelocity;
        if (cabinPosition.z - 1 < spreaderPosition.z) speedZ -= maxSpreaderVelocity;        
        //if (Mathf.Approximately(inputZ, 1)) speedZ = maxSpreaderVelocity;

        //float inputZ = distanceToTravelZKat / (Mathf.Max(speedZ, 0.05f) / acceleration);

        float inputSwing =  (Mathf.Abs((cabinPosition.z -1) - spreaderPosition.z) / (Mathf.Max(speedZ, 0.05f) / acceleration));
        //if(inputZ > 0.95f) inputSwing = Mathf.Min((speedZ / (16 * inputZ)), 1);
        inputSwing = Mathf.Min((speedZ / 16), 1); // afbouwen
        //inputSwing = Mathf.Min((Mathf.Min(maxSpreaderVelocity,16) / (16 * inputZ)), 1);
        inputZ -= inputSwing;
        inputZ = Mathf.Clamp(inputZ, 0, 1);
        inputY *= inputSwing;
        

        if (targetPosition.y < spreaderPosition.y) inputY = -inputY;
        if (targetPosition.z < spreaderPosition.z) inputZ = -inputZ;
       
        if (float.IsNaN(inputY)) inputY = 0;
        if (float.IsNaN(inputZ)) inputZ = 0;

        inputs.y = Mathf.Clamp(inputY, -1, 1);
        inputs.z = Mathf.Clamp(inputZ, -1, 1);
        inputs.y = 0;
     
        return inputs;

    }

    private static Vector3 GetNextPosition(Vector3 spreaderPosition, Vector3 targetPosition)
    {
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
        return targetPosition;
    }

}