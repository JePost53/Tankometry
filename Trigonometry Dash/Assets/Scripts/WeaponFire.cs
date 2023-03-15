using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFire : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public LineRenderer lineRend;
    public Vector3[] coordinates;
    public int index;
    public GameObject projectile;
    public bool firing;
    public int speed;
    public Vector3 firePos;
    public GameObject barrel;
    float timeElapsed;
    public float lerpSpeed;
    public GameObject equationBox;
    public Sprite shell;
    public Sprite missile;
    public Sprite digger;

    // Start is called before the first frame update
    void Start()
    {
        coordinates= new Vector3[lineRend.positionCount];
        lineRend.GetPositions(coordinates);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (firing == true)
        {
            TrajectoryMove();
        }
    }


    public void Shoot()
    {
        if (!projectile)
        {
            if (lineRend.positionCount >= (lineRend.GetComponent<TrajectoryLineScript>().coordRender / lineRend.GetComponent<TrajectoryLineScript>().dotFrequency) / 2)
            {
                coordinates = new Vector3[lineRend.positionCount];
                lineRend.GetPositions(coordinates);
                index = 0;
                GameObject newProj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                newProj.transform.localScale = firePoint.localScale;
                newProj.transform.localScale = new Vector3(-newProj.transform.localScale.x, newProj.transform.localScale.y, newProj.transform.localScale.z);
                projectile = newProj;
                if (equationBox.GetComponentInChildren<ProjType>().type == ProjType.ProjectileType.Shell)
                {
                    newProj.GetComponent<SpriteRenderer>().sprite = shell;
                }
                else if (equationBox.GetComponentInChildren<ProjType>().type == ProjType.ProjectileType.Missile)
                {
                    newProj.GetComponent<SpriteRenderer>().sprite = missile;
                }
                else if (equationBox.GetComponentInChildren<ProjType>().type == ProjType.ProjectileType.Digger)
                {
                    newProj.GetComponent<SpriteRenderer>().sprite = digger;
                }
                timeElapsed = 0;
                firing = true;
                firePos = firePoint.position;
                barrel.GetComponent<Rigidbody2D>().AddForce(10 * barrel.transform.right);
                newProj.transform.position = new Vector3(newProj.transform.position.x, newProj.transform.position.y, 10);

                //barrel.GetComponent<AudioSource>().Play();
            }
            else
            {
                Debug.Log("Waiting for line to render before firing!");
            }
        }

        //for (index = 0; index < coordinates.Length; index = index + 1)
        //{
            //newProj.transform.position = new Vector3(coordinates[index].x, coordinates[index].y, newProj.transform.position.z);
        //}
    }

    public void TrajectoryMove()
    {
        //if(index == 0 && firing == true)
        //{
        //projectile = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        //projectile.transform.localScale = firePoint.localScale;
        //}
        if (index < coordinates.Length - 1)
        {
            if (projectile != null)
            {
                //Debug.Log("Index : " + index + " | PositionIndex : " + coordinates[index]);
                Vector3 point1 = coordinates[index]; //projectile.transform.right; //coordinates[0];
                Vector3 point2 = coordinates[index + 1];
                Vector2 dir = point1 - point2;
                float distance = Vector3.Distance(point1, point2);
                Vector3 startPoint = new Vector3(coordinates[index].x + firePos.x, coordinates[index].y + firePos.y, projectile.transform.position.z);
                Vector3 destinationPoint = new Vector3(coordinates[index + 1].x + firePos.x, coordinates[index + 1].y + firePos.y, projectile.transform.position.z);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                projectile.transform.eulerAngles = new Vector3(0, 0, 180 + angle); //Vector2.SignedAngle(point1, point2));

                if (projectile.transform.position != destinationPoint)
                {
                    //Debug.Log("New Vector3 : " + destinationPoint + " | Distance : " + distance + " | Time Elapsed : " + timeElapsed);
                    projectile.transform.position = Vector3.Lerp(startPoint, destinationPoint, timeElapsed*lerpSpeed/distance);
                    timeElapsed += Time.deltaTime;
                }
                else if (projectile.transform.position == destinationPoint)
                {
                    index += speed;
                    timeElapsed = 0;
                }


                //index += speed;
                //projectile.transform.position = new Vector3(coordinates[index].x + firePos.x, coordinates[index].y + firePos.y, projectile.transform.position.z);
            }
            else
            {
                index = coordinates.Length;
            }
        }
        else
        {
            if(projectile != null)
            {
                projectile.GetComponent<ProjectileScript>().Explode();
            }
        }
    }
}
