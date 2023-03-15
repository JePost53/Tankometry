using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimDirection : MonoBehaviour
{
    public int aDirection = -1;
    public GameObject button;
    public GameObject text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeDirection()
    {
        if(aDirection == -1)
        {
            aDirection = 1;
            text.GetComponent<Text>().text = ">";
        }
        else
        {
            aDirection = -1;
            text.GetComponent<Text>().text = "<";
        }
    }
}
