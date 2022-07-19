using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{
    [SerializeField] List<GameObject> possibleEnemies;
    [SerializeField] List<GameObject> possiblePickups;

    public List<GameObject> spawnedEnemies;
    [SerializeField] List<GameObject> spawnedPickups;

    [SerializeField] List<GameObject> enemySpawnPoints;
    [SerializeField] List<GameObject> pickupSpawnPoints;

    private bool unenteredRoom = true;
    private bool movingToRoom;
    private bool playingRoom;
    private bool clearedRoom;

    private bool didPickupsSpawn;


    // Start is called before the first frame update
    void Start()
    {
        InitializeEnemies(Random.Range(3, 5));
        InitializePickups(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (playingRoom && spawnedEnemies.Count == 0)
        {
            EnablePickups();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (unenteredRoom && other.gameObject.CompareTag("Player"))
        {
            //will only trigger once, when the player enters the room area
            Invoke("EnableEnemies", 0f);

            unenteredRoom = false;
            playingRoom = true;
        }

    }


    void InitializeEnemies(int enemiesToInit)
    {
        if (enemiesToInit <= enemySpawnPoints.Count)
        {
            for (int i = 0; i < enemiesToInit; i++)
            {
                int randSpawnPointIndex = Random.Range(0, enemySpawnPoints.Count);
                int randomEnemyIndex = Random.Range(0, possibleEnemies.Count);
                GameObject newEnemyInstance = Instantiate(possibleEnemies[randomEnemyIndex], enemySpawnPoints[randSpawnPointIndex].transform.position,
                    possibleEnemies[randomEnemyIndex].transform.localRotation);
                newEnemyInstance.gameObject.name = newEnemyInstance.gameObject.name + " " + i;
                spawnedEnemies.Add(newEnemyInstance);
                enemySpawnPoints.RemoveAt(randSpawnPointIndex);
            }
        }
    }
    void EnableEnemies()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            spawnedEnemies[i].SetActive(true);
            spawnedEnemies[i].GetComponent<EnemyBehaviorA>().SpawnEnableEnemy();
        }
    }

    void InitializePickups(int pickupsToInit)
    {
        if (pickupsToInit <= pickupSpawnPoints.Count)
        {
            for (int i = 0; i < pickupsToInit; i++)
            {
                int randSpawnPointIndex = Random.Range(0, pickupSpawnPoints.Count);
                int randPickupIndex = Random.Range(0, possiblePickups.Count);
                GameObject newPickupInstance = Instantiate(possiblePickups[randPickupIndex], pickupSpawnPoints[randSpawnPointIndex].transform.position,
                    possiblePickups[randPickupIndex].transform.localRotation);
                spawnedPickups.Add(newPickupInstance);
                pickupSpawnPoints.RemoveAt(randSpawnPointIndex);
            }
        }
    }
    void EnablePickups()
    {
        if (!didPickupsSpawn)
        {
            didPickupsSpawn = true; //fire once
            for (int i = 0; i < spawnedPickups.Count; i++)
            {
                spawnedPickups[i].SetActive(true);
                spawnedPickups[i].GetComponent<PickupBehavior>().SpawnPickup();
            }
        }
    }



}
