using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPFollow : MonoBehaviour
{

    public Transform targetObject;
    public int yOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowObject();
    }

    void FollowObject()
    {
        //Vector3 screenPos = Camera.main.WorldToScreenPoint(targetObject.position);
        transform.position = new Vector3(targetObject.position.x, targetObject.position.y + yOffset, targetObject.position.z); //new Vector3(screenPos.x, screenPos.y + yOffset, screenPos.z);
    }
}
