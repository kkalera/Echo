using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkewManager : MonoBehaviour
{
    [Tooltip("Rotation speed in degree per second")]
    [SerializeField] float rotationSpeed = 1;
    [SerializeField] float maxRotation = 5;
    
    void Update()
    {
        Echo.Utils.ClearLogConsole();

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Quaternion newAngle = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.down * rotationSpeed * Time.deltaTime);
            if (newAngle.eulerAngles.y > maxRotation && newAngle.eulerAngles.y < 360 - maxRotation) newAngle = Quaternion.Euler(new Vector3(0, 360 - maxRotation, 0));
            transform.rotation = newAngle;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Quaternion newAngle = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * rotationSpeed * Time.deltaTime);
            if (newAngle.eulerAngles.y > maxRotation && newAngle.eulerAngles.y < 360 - maxRotation) newAngle = Quaternion.Euler(new Vector3(0, maxRotation, 0));
            transform.rotation = newAngle;
        }

        
        Debug.Log(transform.rotation.eulerAngles);
    }
}
