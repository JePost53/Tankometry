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
    public GameObject[] maps;

    public int currentTurn = 0;

    public CamMover camMover;
    public WeaponFire weaponFire;
    public GameObject equationBox;
    public GameObject currentMap;

    public GameObject lastTank;



    private void Awake()
    {
        NewMap();
    }


    void Start()
    {
        //camMover.player = tanks[currentTurn].transform;
    }


    public void NewMap()
    {
        GameObject map = maps[Random.Range(0, maps.Length)];
        currentMap.SetActive(false);
        DespawnTanks();

        currentMap = map;
        map.SetActive(true);

        spawnPoints = new Vector2[tankCount];
        Transform spawnPointsParent = currentMap.transform.Find("SpawnPositions");
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Debug.Log("spawnpoint: " + spawnPointsParent.name);
            spawnPoints[i] = spawnPointsParent.GetChild(i).transform.position;
        }

        SpawnTanks();

        Invoke("NewTurn", 2);
    }


     // Spawn tanks in at designated position (currently only called on awake() )
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

    void DespawnTanks()
    {
        if (tanks == null)
            return;

        foreach (var tank in tanks)
        {
            if (tank != null)
                Destroy(tank);
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
        tanks[currentTurn].GetComponentInChildren<TrajectoryLineScript>().TurnTurret(); 
    }

    // This gets called every time a new turn begins
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

        if (lastTank == tanks[currentTurn].gameObject)
        {
            RoundOver(lastTank);
        }
        lastTank = tanks[currentTurn].gameObject;

        tanks[currentTurn].GetComponentInChildren<TankMove>().MoveActivate(false);
        camMover.player = tanks[currentTurn].transform;
        tanks[currentTurn].GetComponentInChildren<LineRenderer>(true).gameObject.SetActive(true);
        StartCoroutine(tanks[currentTurn].GetComponentInChildren<LineRenderer>(true).transform.GetComponent<TrajectoryLineScript>().LineGenerator2());

        weaponFire.lineRend = tanks[currentTurn].GetComponentInChildren<LineRenderer>(); //transform.Find("LineRenderThing").gameObject.GetComponent<LineRenderer>();
        weaponFire.barrel = tanks[currentTurn].transform.Find("Hull/HullB/Turret/TurretB/Barrel").gameObject;
        weaponFire.firePoint = tanks[currentTurn].transform.Find("Hull/HullB/Turret/TurretB/Barrel/FirePoint");
    }

    public void RoundOver(GameObject winner)
    {
        Debug.Log("Game Over!!!!!");
        Invoke("NewMap", 5);
    }

}
