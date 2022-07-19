using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{
    [SerializeField] List<GameObject> possibleEnemies;
    [SerializeField] List<GameObject> possiblePickups;

    public List<GameObject> spawnedEnemies;
    [SerializeField] List<GameObject> spawnedPickups;


    //[SerializeField] List<GameObject> enemyPool;

    [SerializeField] List<GameObject> enemySpawnPoints;
    [SerializeField] List<GameObject> pickupPool;
    [SerializeField] List<GameObject> pickupSpawnPoints;

    private bool unenteredRoom = true;
    private bool movingToRoom;
    private bool playingRoom;
    private bool clearedRoom;

    private bool didPickupsSpawn;



    // Start is called before the first frame update
    void Start()
    {
        //SpawnPickups(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (playingRoom && spawnedEnemies.Count == 0)
        {
            EnablePickups();
        }
    }

    /*
    void SpawnRoomEnemies(int enemiesToSpawn)
    {
        if (enemiesToSpawn <= enemySpawnPoints.Count)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                int randSpawnPointIndex = Random.Range(0, enemySpawnPoints.Count);
                int randEnemyIndex = Random.Range(0, enemyPool.Count);
                enemyPool[randEnemyIndex].transform.position = enemySpawnPoints[randSpawnPointIndex].transform.position;
                enemyPool[randEnemyIndex].GetComponent<EnemyBehaviorA>().SpawnEnemy();
                //enemyPool[randEnemyIndex].SetActive(true);
                //if I'm going to have different enemy classes inheriting from a base class
                //I need to have a way to check if they have a script inheriting from the base class
                //rather than doing a series of if statements checking for each class script's name
                //then is there a way to call a same-named method for each enemy instance?
                //https://answers.unity.com/questions/1717478/find-script-inherits-from-a-specific-base-class.html
                enemyPool.RemoveAt(randEnemyIndex);
                enemySpawnPoints.RemoveAt(randSpawnPointIndex);
            }
        }
        else
        {
            Debug.LogWarning("Spawning more enemies than there are enemy spawn points");
        }
    }*/

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
            spawnedEnemies[i].GetComponent<EnemyBehaviorA>().SpawnEnemy();
        }
        /*
        foreach (GameObject enemy in spawnedEnemies)
        {
            int i = 0;
            spawnedEnemies[i].SetActive(true);
            spawnedEnemies[i].GetComponent<EnemyBehaviorA>().SpawnEnemy();
            i ++;
        } */
    }

    /*
    void SpawnPickups(int pickupsToSpawn)
    {
        if (pickupsToSpawn <= pickupSpawnPoints.Count)
        {
            for (int i = 0; i < pickupsToSpawn; i++)
            {
                int randSpawnPointIndex = Random.Range(0, pickupSpawnPoints.Count);
                int randPickupIndex = Random.Range(0, pickupPool.Count);
                pickupPool[randPickupIndex].transform.position = pickupSpawnPoints[randSpawnPointIndex].transform.position;
                pickupPool[randPickupIndex].GetComponent<PickupBehavior>().SpawnPickup();

                pickupPool.RemoveAt(randPickupIndex);
                pickupSpawnPoints.RemoveAt(randSpawnPointIndex);
            }
        }
        else
        {
            Debug.LogWarning("Spawning more pickups than there are pickup spawn points");
        }
    } */

    void InitializePickups(int pickupsToInit)
    {
        if (pickupsToInit <= pickupSpawnPoints.Count)
        {
            for (int i = 0; i < pickupsToInit; i++)
            {
                int randSpawnPointIndex = Random.Range(0, pickupSpawnPoints.Count);
                int randPickupIndex = Random.Range(0, possiblePickups.Count);
                GameObject newPickupInstance = Instantiate(possiblePickups[randPickupIndex], pickupSpawnPoints[randPickupIndex].transform.position,
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

    private void OnTriggerEnter(Collider other)
    {
        if (unenteredRoom && other.gameObject.CompareTag("Player"))
        {
            //will only trigger once, when the player enters the room area
            //SpawnRoomEnemies(Random.Range(3, 5));
            InitializeEnemies(Random.Range(3, 5));
            InitializePickups(Random.Range(2, 4));
            //InitializePickups(3);

            unenteredRoom = false;
            playingRoom = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EnableEnemies();
        }
    }
}
