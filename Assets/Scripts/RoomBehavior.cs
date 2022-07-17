using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{
    [SerializeField] List<GameObject> enemyPool;
    [SerializeField] List<GameObject> spawnPoints;

    private bool roomHasStarted;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnRoomEnemies(int enemiesToSpawn)
    {
        if (enemiesToSpawn <= spawnPoints.Count)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                int randSpawnPointIndex = Random.Range(0, spawnPoints.Count);
                int randEnemyIndex = Random.Range(0, enemyPool.Count);
                enemyPool[randEnemyIndex].transform.position = spawnPoints[randSpawnPointIndex].transform.position;
                enemyPool[randEnemyIndex].SetActive(true);
                enemyPool.RemoveAt(randEnemyIndex);
                spawnPoints.RemoveAt(randSpawnPointIndex);
            }
        }
        else
        {
            Debug.LogWarning("Spawning more enemies than there are enemy spawn points");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!roomHasStarted && other.gameObject.CompareTag("Player"))
        {
            //will only trigger once, when the player enters the room
            roomHasStarted = true;
            //method to spawn enemies
            SpawnRoomEnemies(3);
        }

    }
}
