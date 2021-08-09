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
    public static Vector3 GetInputsSwing(Vector3 targetPosition, Vector3 spreaderPosition, Vector3 cabinPosition, Vector3 currentSpreaderSpeed, Vector3 currentKatSpeed,Vector3 spreaderAngularVelocity, float acceleration)
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
        float inputY = distanceToTravelY / (Mathf.Max(Mathf.Abs(currentSpreaderSpeed.y), 0.05f) / (acceleration));



        float angle = Vector3.SignedAngle(new Vector2(cabinPosition.z + 1, cabinPosition.y), new Vector2(spreaderPosition.z, cabinPosition.y), Vector3.up);
        float length = Vector3.Distance(spreaderPosition, cabinPosition);

        float maxSpreaderVelocity = Mathf.Sqrt(2 * length * 9.81f *(1 - Mathf.Cos(angle)));


        float pendulumHeight = length - Mathf.Abs(cabinPosition.y - spreaderPosition.y);
        float angleToSwingLeft = Mathf.Pow(length ,2) * Mathf.Pow(Mathf.Abs(currentSpreaderSpeed.z), 2);
        angleToSwingLeft /= 9.81f * length;
        angleToSwingLeft /= Mathf.Rad2Deg;


        float distanceToSwingLeft = Mathf.Pow(length, 2) + Mathf.Pow(length, 2) - 2 * length * length * Mathf.Cos(angleToSwingLeft);
        distanceToSwingLeft = Mathf.Sqrt(distanceToSwingLeft);
        
        //float distanceToTravelZ = Mathf.Min(Mathf.Abs(targetPosition.z - spreaderPosition.z), 4/acceleration);
        float distanceToTravelZ = Mathf.Min(Mathf.Abs(targetPosition.z - cabinPosition.z + 1), 4/acceleration);
        //distanceToTravelZ += spreaderPosition.z - (cabinPosition.z + 1);

        //float speedZ = currentKatSpeed.z + currentSpreaderSpeed.z;
        float speedZ = Mathf.Abs(currentKatSpeed.z) ;
        
        if (spreaderPosition.z - 1 < cabinPosition.z && targetPosition.z < spreaderPosition.z)
        {            
            distanceToTravelZ -= Mathf.Abs(spreaderPosition.z - 1 - cabinPosition.z);
            //speedZ -= Mathf.Abs(maxSpreaderVelocity - Mathf.Abs(currentSpreaderSpeed.z));
            speedZ -= maxSpreaderVelocity;
            
        }
        if (spreaderPosition.z - 1 > cabinPosition.z && targetPosition.z < spreaderPosition.z)
        {
            distanceToTravelZ += Mathf.Abs(spreaderPosition.z - 1 - cabinPosition.z);
            //speedZ += Mathf.Abs(maxSpreaderVelocity - Mathf.Abs(currentSpreaderSpeed.z));
            speedZ += maxSpreaderVelocity;
            
        }
        if (spreaderPosition.z - 1 < cabinPosition.z && targetPosition.z > spreaderPosition.z)
        {
            distanceToTravelZ += Mathf.Abs(spreaderPosition.z - 1 - cabinPosition.z);
            //speedZ += Mathf.Abs(maxSpreaderVelocity - Mathf.Abs(currentSpreaderSpeed.z));
            speedZ += maxSpreaderVelocity;
            
        }
        if (spreaderPosition.z - 1 > cabinPosition.z && targetPosition.z > spreaderPosition.z)
        {
            distanceToTravelZ -= Mathf.Abs(spreaderPosition.z - 1 - cabinPosition.z);
            //speedZ -= Mathf.Abs(maxSpreaderVelocity - Mathf.Abs(currentSpreaderSpeed.z));
            speedZ -= maxSpreaderVelocity ;
            
        }

        distanceToTravelZ = Mathf.Min(distanceToTravelZ, 4 / acceleration);
        
        float inputZ = distanceToTravelZ / (Mathf.Max(Mathf.Abs(currentKatSpeed.z), 0.01f) / acceleration) * (4  / Mathf.Clamp(Mathf.Abs(speedZ), 4f, 16f));
        inputY *= (4 / Mathf.Clamp(Mathf.Abs(maxSpreaderVelocity), 4f, 16f));

        Utils.ClearLogConsole();
        Debug.Log(speedZ);
        Debug.Log(angle);
        Debug.Log(maxSpreaderVelocity);
        Debug.Log(inputZ);

        /*if (distanceToTravelZ < 16) inputZ = distanceToTravelZ / (Mathf.Max(Mathf.Abs(currentKatSpeed.z), 0.01f) / acceleration) /
                    (4 / acceleration / Mathf.Clamp(maxSpreaderVelocity, 4f, 4 / acceleration));*/


        if (targetPosition.y < spreaderPosition.y) inputY = -inputY;
        if (targetPosition.z < spreaderPosition.z && targetPosition.z < cabinPosition.z + 1) inputZ = -inputZ;
        if (targetPosition.z < spreaderPosition.z && targetPosition.z > cabinPosition.z + 1) inputZ = -inputZ;
        if (targetPosition.z > spreaderPosition.z && targetPosition.z < cabinPosition.z + 1) inputZ = +inputZ;
        if (targetPosition.z > spreaderPosition.z && targetPosition.z > cabinPosition.z + 1) inputZ = +inputZ;

        if (float.IsNaN(inputY)) inputY = 0;
        if (float.IsNaN(inputZ)) inputZ = 0;

        inputs.y = Mathf.Clamp(inputY, -1, 1);
        inputs.z = Mathf.Clamp(inputZ, -1, 1);
        //inputs.y = 0;

        return inputs;

    }

}