using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjType : MonoBehaviour
{
    public enum ProjectileType
    {
        Shell,
        Missile,
        Digger
    }

    public ProjectileType type;

    public Sprite shell;
    public Sprite missile;
    public Sprite digger;

    private void Start()
    {
        if(type == ProjectileType.Shell)
        {
            GetComponent<Image>().sprite = shell;
        }
        else if (type == ProjectileType.Missile)
        {
            GetComponent<Image>().sprite = missile;
        }
        else if (type == ProjectileType.Digger)
        {
            GetComponent<Image>().sprite = digger;
        }
    }

}
