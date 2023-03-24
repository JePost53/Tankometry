using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float speed = 1f;
    public Rigidbody2D rb;
    public GameObject lineObject;
    public LineRenderer lineRend;
    public Vector3[] coordinates;
    public ParticleSystem impact;
    public bool hit;
    public float blastRadius;
    public float force;
    public GameObject terrain;
    public GameObject crater;
    public bool pen;
    public float penTimer = 0.01f;
    public AudioSource missileSound;
    public Sprite missile;
    public Sprite shell;
    public Sprite digger;
    public ParticleSystem missileTrail;
    public ParticleSystem diggerTrail;
    public AudioSource shellFire;
    public AudioSource missileFire;
    public bool digging;

    public float shellDamage;
    
    // Start is called before the first frame update
    void Start()
    {
        terrain = GameObject.Find("Terrain");
        if(transform.GetComponent<SpriteRenderer>().sprite == missile)
        {
            missileSound.Play();
            missileTrail.Play();
            missileFire.Play();
        }
        else if(transform.GetComponent<SpriteRenderer>().sprite == shell)
        {
            shellFire.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(impact.isStopped && hit ==true)//impact.main.duration == impact.time)
        {
            Destroy(gameObject);
        }
        if(pen == true)
        {
            penTimer -= Time.deltaTime;
            //Debug.Log("PenTimer: " + penTimer);
            if(penTimer < 1)
            {
                //Debug.Log("BOOOOOM!!");
                penTimer = 999;
                Explode();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (transform.GetComponent<SpriteRenderer>().sprite == digger)
        {
            if (hitInfo.gameObject.tag == "Ground")
            {
                //Debug.Log("Digging is true! We have struck DIORT!!!!");
                digging = true;
                diggerTrail.Play();
            }
        }
        else
        {
            pen = true;
        }
        //float time = Time.fixedTime;
        for (float i = 1; i < 0; i -= 0)//Time.deltaTime)
        {
            //i -= Time.deltaTime;
            //Debug.Log("TIME: ");
            if (i < 0.1)
            {
                //Explode();
            }
        }
        //Debug.Log(hitInfo);
        //impact.Play();
        //gameObject.transform.localScale = new Vector3(0, 0, 0);
        //hit = true;
        //Instantiate(crater, transform.position, transform.rotation);
        //Explode();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Exited some collider: " + collision.name + " | Digging true? " + digging + " | Other tag? " + collision.gameObject.tag + " | hotel? trivago.");
        if(digging == true && collision.gameObject.tag == "Ground")
        {
            Debug.Log("WE HAVE EXITED THE DIORT!! EXPLOADE!!!");
            diggerTrail.Stop();
            Explode();
        }
    }

    public void Explode()
    {
        if (hit == false)
        {
            //Debug.Log("BOOM");
            transform.GetComponent<AudioSource>().Play();
            impact.Play();
            missileSound.loop = false;
            missileSound.Stop();
            gameObject.transform.localScale = new Vector3(0, 0, 0);
            hit = true;
            Instantiate(crater, transform.position, transform.rotation);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);

            foreach (Collider2D nearObject in colliders)
            {
                Debug.Log("Projectile impact against object : " + nearObject);

                if (nearObject != transform.gameObject)
                {
                    Vector2 dir = nearObject.transform.position - transform.position;
                    float distance = Mathf.Clamp(Vector2.Distance(nearObject.transform.position, transform.position), 0.5f, 100);

                    Rigidbody2D rb = nearObject.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.AddForce(dir * force / distance);

                    }
                    if (nearObject.transform.parent)
                    {
                        if (nearObject.transform.parent.GetComponentInChildren<HealthBarScript>())
                        {
                            if (distance < 1)
                            {
                                distance = 1;
                            }
                            Debug.Log("Shell Impact against object: " + nearObject.name + " | Distance is : " + distance + " | Damage is : " + shellDamage / distance);
                            nearObject.transform.parent.GetComponentInChildren<HealthBarScript>().SetHealth(shellDamage / distance);
                        }
                    }
                }
            }

            StartCoroutine("NextTurn");
        }
    }

    IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(3);

        GameObject gameManager = GameObject.Find("_GameManager");

        gameManager.GetComponent<GameManagerScript>().NewTurn();

        //Destroy(gameObject);
    }

}
