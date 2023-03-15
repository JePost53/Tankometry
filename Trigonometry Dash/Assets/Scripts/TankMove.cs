using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankMove : MonoBehaviour
{
    public GameObject tank;
    public bool moveActive;
    public GameObject expressionObject;
    public float speed;
    public float direction;
    public float movement;
    public Rigidbody2D rBody;
    public WheelJoint2D wheel1;
    public WheelJoint2D wheel2;
    public WheelJoint2D wheel3;
    public WheelJoint2D wheel4;
    public WheelJoint2D wheel5;
    public WheelJoint2D wheel6;
    public Transform turret;
    public GameObject lineObject;

    // Start is called before the first frame update
    void Start()
    {
        rBody = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (moveActive == true)
        {
            direction = -Input.GetAxisRaw("Horizontal");
            movement = direction * speed;
        }
        else
        {
            direction = 0;
            movement = 0;
        }
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            //turret.localScale = new Vector3(turret.transform.localScale.x * -1, 1, 1);
        }
    }

    public void MoveActivate(bool deactivate)
    {
        if(deactivate == true)
        {
            moveActive = false;
            //expressionObject.GetComponent<InputField>().interactable = true;
        }
        else if(deactivate == false)
        {
            moveActive = true;
        }
        else if (moveActive == true)
        {
            moveActive = false;
            //expressionObject.GetComponent<InputField>().interactable = true;
        }
        else
        {
            moveActive = true;
            //expressionObject.GetComponent<InputField>().interactable = false;
        }
    }

    void FixedUpdate()
    {
        //GetPressed();
        //MoveTank();
        MoveTank2();
    }

    public void MoveTank2()
    {
        JointMotor2D motor = new JointMotor2D { motorSpeed = movement, maxMotorTorque = 10000 };
        wheel1.motor = motor;
        wheel2.motor = motor;
        wheel3.motor = motor;
        wheel4.motor = motor;
        wheel5.motor = motor;
        wheel6.motor = motor;
        if ((direction == 1 && rBody.velocity.x < 0)|| direction == -1 && rBody.velocity.x > 0)
        {
            if(transform.localScale.x != direction || direction == 0)
            {
                Debug.Log("Hull turning, turning trajline too!");
                transform.parent.GetComponentInChildren<TrajectoryLineScript>().ChangeDirection();
            }
            //Debug.Log("velocity:" + rBody.velocity);
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }

    public void GetPressed()
    {
        //direction = 0;
        if (Input.GetKey(KeyCode.D))
        {
            direction = 1;
            //Debug.Log("Trajline : " + transform.parent.GetComponentInChildren<TrajectoryLineScript>() + " and adirection : " + transform.parent.GetComponentInChildren<TrajectoryLineScript>().aDirection);
            //transform.parent.GetComponentInChildren<TrajectoryLineScript>().aDirection = 1;
        }
        else if(Input.GetKey(KeyCode.A))
        {
            direction = -1;
            //Debug.Log("Trajline : " + transform.parent.GetComponentInChildren<TrajectoryLineScript>() + " and adirection : " + transform.parent.GetComponentInChildren<TrajectoryLineScript>().aDirection);
            //transform.parent.GetComponentInChildren<TrajectoryLineScript>().aDirection = -1;
        }
        else
        {
            direction = 0;
        }
    }

    public void MoveTank()
    {
        if(direction == 1)
        {
            transform.GetComponent<Rigidbody2D>().AddForce(tank.transform.right * speed);
        }
        if(direction == -1)
        {
            transform.GetComponent<Rigidbody2D>().AddForce(tank.transform.right * -speed);
        }
        if(direction == 0)
        {
            transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,0), ForceMode2D.Impulse);
        }
    }
}
