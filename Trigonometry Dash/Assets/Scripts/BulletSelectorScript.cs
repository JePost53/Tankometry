using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletSelectorScript : MonoBehaviour
{
    //this is property of jake post yesyes
    //These variables are a different types of shells

    public GameObject _X; 

    public void BlankX()
    {
        if(_X.activeSelf == true)
        {
            _X.SetActive(false);
        }
        else
        {
            _X.SetActive(true);
        }
    }
}