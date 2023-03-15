using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    public Transform tank;
    public Vector3 angle;
    //public Transform wheelBone;

    // Start is called before the first frame update
    void Start()
    {
        angle = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = angle;
    }
}
