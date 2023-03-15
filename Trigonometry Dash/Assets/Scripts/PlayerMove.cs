using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameObject player;
    public GameObject lRender;
    public LineRenderer lRendComp;
    public Vector3[] positions;
    public bool movePlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        lRendComp = lRender.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    public void OnButtonPress()
    {
        //LineRenderer lRendComp = lRender.GetComponent<LineRenderer>();
        positions = new Vector3[lRendComp.positionCount];
        lRendComp.GetPositions(positions);

        movePlayer = true;

        //for(int i=0; i < positions.Length; i++)
        //{
            //player.transform.position = positions[i] * Time.fixedDeltaTime;
        //}
    }

    public void MovePlayer()
    {
        if(movePlayer == true && lRendComp.positionCount > 1)
        {
            if(lRendComp.GetPosition(1).x != 0)
            {
                player.transform.position = lRendComp.GetPosition(1); //positions[1]; //* Time.deltaTime;
            }
            else
            {
                movePlayer = false;
            }
        }
        else
        {
            movePlayer = false;
        }
    }
}
