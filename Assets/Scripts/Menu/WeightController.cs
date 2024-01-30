using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class WeightController : MonoBehaviour
{
    HingeJoint joint;
    public float speed = 80;
    public KeyCode upButton = KeyCode.Space; // �������ϵİ�ť�ǿո��
    public KeyCode downButton = KeyCode.LeftShift; // �������µİ�ť����Shift��

    // Use this for initialization
    void Start()
    {
        joint = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    JointMotor motor = joint.motor;

    //    if (Input.GetKeyDown(upButton))
    //    {
    //        motor.targetVelocity = speed;
    //    }
    //    else if (Input.GetKeyDown(downButton))
    //    {
    //        motor.targetVelocity = -speed;
    //    }
    //    else
    //    {
    //        motor.targetVelocity = 0;
    //    }
    //    joint.motor = motor;
    //}

    public void OnUpButtonClick()
    {
        JointMotor motor = joint.motor;
        motor.targetVelocity = speed;
        joint.motor = motor;
    }

    // This method will be called when the DownButton is clicked
    public void OnDownButtonClick()
    {
        JointMotor motor = joint.motor;
        motor.targetVelocity = -speed;
        joint.motor = motor;
    }

    public void OnStopButtonClick()
    {
        JointMotor motor = joint.motor;
        motor.targetVelocity = 0;
        joint.motor = motor;
    }
}