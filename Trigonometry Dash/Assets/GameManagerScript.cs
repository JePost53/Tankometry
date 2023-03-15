using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    public GameObject tankPrefab;
    GameObject[] tanks;
    public int tankCount;

    public Vector2[] spawnPoints;

    public int currentTurn = 0;

    public CamMover camMover;
    public WeaponFire weaponFire;
    public GameObject equationBox;

    // Start is called before the first frame update
    void Start()
    {
        //camMover.player = tanks[currentTurn].transform;
        NewTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void Awake()
    {
        SpawnTanks();
    }

    void SpawnTanks()
    {
        tanks = new GameObject[tankCount];
        for(int i=0; i < tankCount; i++)
        {
            GameObject instance = Instantiate(tankPrefab);
            tanks[i] = instance;
            instance.transform.position = spawnPoints[i];

            instance.GetComponentInChildren<TrajectoryLineScript>().equationBox = equationBox;

            Transform nameBar = instance.GetComponentInChildren<HealthBarScript>().transform.Find("PlayerLabel");
            nameBar.GetComponentInChildren<Text>().text = "Player" + (i + 1);

        }
    }

    public void CallLineGenerator()
    {
        StartCoroutine(tanks[currentTurn].GetComponentInChildren<TrajectoryLineScript>().LineGenerator2());
    }

    public void CallMoveScript()
    {
        tanks[currentTurn].GetComponentInChildren<TankMove>().MoveActivate(false);
    }

    public void CallDirectionChange()
    {
        //Debug.Log("Do I crash?");
        tanks[currentTurn].GetComponentInChildren<TrajectoryLineScript>().TurnTurret(); //ChangeDirection();
    }

    public void NewTurn()
    {
        Debug.Log("LineRend : " + tanks[currentTurn].GetComponentInChildren<LineRenderer>(true));
        tanks[currentTurn].GetComponentInChildren<LineRenderer>(true).transform.gameObject.SetActive(false);
        tanks[currentTurn].GetComponentInChildren<TankMove>().MoveActivate(true);

        currentTurn++;
        if (currentTurn >= tankCount)
        {
            currentTurn = 0;
        }
        while (tanks[currentTurn].GetComponentInChildren<Slider>().value == 0)
        {
            currentTurn++;
            if (currentTurn >= tankCount)
            {
                currentTurn = 0;
            }
        }

        tanks[currentTurn].GetComponentInChildren<TankMove>().MoveActivate(false);
        camMover.player = tanks[currentTurn].transform;
        tanks[currentTurn].GetComponentInChildren<LineRenderer>(true).gameObject.SetActive(true);
        StartCoroutine(tanks[currentTurn].GetComponentInChildren<LineRenderer>(true).transform.GetComponent<TrajectoryLineScript>().LineGenerator2());

        weaponFire.lineRend = tanks[currentTurn].GetComponentInChildren<LineRenderer>(); //transform.Find("LineRenderThing").gameObject.GetComponent<LineRenderer>();
        weaponFire.barrel = tanks[currentTurn].transform.Find("Hull/HullB/Turret/TurretB/Barrel").gameObject;
        weaponFire.firePoint = tanks[currentTurn].transform.Find("Hull/HullB/Turret/TurretB/Barrel/FirePoint");
    }

}
