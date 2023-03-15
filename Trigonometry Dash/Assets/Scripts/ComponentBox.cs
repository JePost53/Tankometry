using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentBox : MonoBehaviour
{
    public enum Type
    {
        Container,
        Operator,
        Constant,
        Input,
        Variable
    };

    public Type type;
    public float value;
    public bool hasSolved;

    public enum OperatorType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Exponent
    };
    public OperatorType operatorType;

    public void Unsolve()
    {
        transform.GetComponent<ComponentBox>().hasSolved = false;
        for (int i= 1; i < transform.GetComponentsInChildren<ComponentBox>().Length; i++) //foreach (Transform child in transform)
        {
            if (transform.GetComponentsInChildren<ComponentBox>()[i].GetComponent<ComponentBox>() != null)
            {
                transform.GetComponentsInChildren<ComponentBox>()[i].GetComponent<ComponentBox>().hasSolved = false;
            }
        }
        //Debug.Log("Problems unsolved!");
    }
}
