using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour
{
    private HingeJoint spool1;
    private HingeJoint spool2;
    private HingeJoint spool3;
    private HingeJoint spool4;

    public Kat Kat { get; private set; }
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        Kat = GetComponentInChildren<Kat>();
        HingeJoint[] joints = GetComponentsInChildren<HingeJoint>();
        spool1 = joints[0];
        spool2 = joints[2];
        spool3 = joints[1];
        spool4 = joints[3];
    }

    public void Hijs(float value)
    {
        JointMotor motor = spool1.motor;
        JointMotor motor2 = spool2.motor;
        motor.targetVelocity = value;
        motor2.targetVelocity = -value;

        spool1.motor = motor;
        spool2.motor = motor;
        spool3.motor = motor2;
        spool4.motor = motor2;
    }

}
