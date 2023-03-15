using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Transform targetObject;
    public Slider slider;
    public Vector3 offset;

    public ParticleSystem deathExplode;
    public ParticleSystem deathFire;
    public Transform tank;

    // Update is called once per frame
    void Update()
    {
        FollowObject();
    }

    public void SetHealth(float damage)
    {
        slider.value -= damage;
        if(slider.value == 0)
        {
            DeathExplode();
        }
    }

    void FollowObject()
    {
        transform.position = Camera.main.WorldToScreenPoint(targetObject.position + offset); //(targetObject.position.x, targetObject.position.y + yOffset);
    }

    void DeathExplode()
    {
        deathExplode.Play();
        deathFire.Play();
        foreach(SpriteRenderer child in tank.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            child.color = Color.black;
            //if(child.GetComponent<SpriteRenderer>())
            //{
                //child.GetComponent<SpriteRenderer>().color = Color.black;
            //}
        }
    }
}
