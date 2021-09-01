using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnvironment : MonoBehaviour
{
    [SerializeField] GameObject craneObject;
    [SerializeField] Transform container;
    [SerializeField] [Range(-20,35)] float containerPosition;
    [SerializeField] [Range(-20, 35)] float cranePosition;
    [SerializeField] [Range(0, 25)] float spreaderHeight;
    [SerializeField] bool winchEnabled = false;


    private ICrane crane;

    // Start is called before the first frame update
    void Start()
    {
        crane = craneObject.GetComponent<ICrane>();
        crane.ResetToPosition(new Vector3(0, spreaderHeight, cranePosition));
        container.localPosition = new Vector3(0, 0, containerPosition);
        crane.WinchMovementDisabled = !winchEnabled;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputs = AutoPilot.GetInputsSwing(
            container.localPosition,
            crane.SpreaderPosition,
            crane.CabinPosition,
            crane.SpreaderVelocity,
            crane.CabinVelocity,
            crane.SpreaderRotation,
            1f
            );

        crane.MoveCabin(inputs.z);
        crane.MoveWinch(inputs.y);
    }    
}
