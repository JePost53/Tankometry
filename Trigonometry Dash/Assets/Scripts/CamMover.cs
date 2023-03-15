using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMover : MonoBehaviour
{
    public Transform player;
    //public Transform rLine;
    public Vector3 targetCamPos;
    public float timeElapsed;
    public float panSpeed;
    Vector3 oldCamPos;
    public float distanceForPan;

    // Start is called before the first frame update
    void Start()
    {
        oldCamPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        camMid();
    }

    private void camMid()
    {
        Vector3 playerPos = player.GetChild(0).position;

        targetCamPos = new Vector3(playerPos.x, playerPos.y, transform.position.z);
        transform.position = targetCamPos;
        //if (targetCamPos != new Vector3(playerPos.x, playerPos.y, transform.position.z))
        //{
        //targetCamPos = new Vector3(playerPos.x, playerPos.y, transform.position.z);
        //timeElapsed = 0;
        //oldCamPos = transform.position;
        //}

        //if(transform.position != targetCamPos)  //if(Vector3.Distance(oldCamPos, transform.position) > distanceForPan)
        //{
        //transform.position = Vector3.Lerp(oldCamPos, targetCamPos, timeElapsed * panSpeed / Vector3.Distance(oldCamPos, targetCamPos));
        //timeElapsed += Time.deltaTime;
        //}
        //else
        //{
        //timeElapsed = 0;
        //oldCamPos = targetCamPos;
        //}
        //else
        //{
        //transform.position = targetCamPos;
        //}
        //transform.position = targetCamPos;
    }
}
