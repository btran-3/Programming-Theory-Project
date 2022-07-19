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

    private AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;

    private bool unenteredRoom = true;
    //private bool movingToRoom;
    private bool playingRoom;
    private bool clearedRoom;

    private bool didPickupsSpawn;


    // Start is called before the first frame update
    void Start()
    {
        InitializeEnemies(Random.Range(3, 5));
        InitializePickups(3);
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playingRoom && spawnedEnemies.Count == 0) //killed all enemies
        {
            audioSource.PlayOneShot(audioClips[0]);
            playingRoom = false;
            clearedRoom = true;
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
                Vector3 randomPosOffset = new Vector3(RNGf(-0.5f, 0.5f), RNGf(-1, 1), RNGf(-0.5f, 0.5f));

                int randSpawnPointIndex = Random.Range(0, pickupSpawnPoints.Count);
                int randPickupIndex = Random.Range(0, possiblePickups.Count);
                GameObject newPickupInstance = Instantiate(possiblePickups[randPickupIndex],
                    pickupSpawnPoints[randSpawnPointIndex].transform.position + randomPosOffset,
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

    float RNGf(float min, float max)
    {
        float a = Random.Range(min, max);
        return a;
    }

}
