using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomBehavior : MonoBehaviour
{
    [SerializeField] private string roomMusicType;

    [SerializeField] List<GameObject> possibleEnemies;
    [SerializeField] List<GameObject> possiblePickups;

    public List<GameObject> spawnedEnemies;
    [SerializeField] List<GameObject> spawnedPickups;

    [SerializeField] List<GameObject> enemySpawnPoints;
    [SerializeField] List<GameObject> pickupSpawnPoints;

    [SerializeField] GameObject doorTop;
    [SerializeField] GameObject doorBottom;

    [SerializeField] GameObject playerStartPos;

    private AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;

    [SerializeField] private Vector2 enemiesToSpawnExclInt;
    [SerializeField] private Vector2 pickupsToSpawnExclInt;

    [SerializeField] private Collider colliderA;

    private bool unenteredRoom = true;
    //private bool movingToRoom;
    private bool playingRoom;
    //private bool clearedRoom;

    private bool didPickupsSpawn;
    private Vector3 doorDefaultScale;

    public string pub_roomMusicType
    {
        get { return roomMusicType; }
    }
    public Vector3 pub_playerStartPos
    {
        get { return playerStartPos.transform.position; }
    }


    void Start()
    {
        InitializeEnemies(((int)Random.Range(enemiesToSpawnExclInt.x, enemiesToSpawnExclInt.y)));
        InitializePickups(((int)Random.Range(pickupsToSpawnExclInt.x, pickupsToSpawnExclInt.y)));
        audioSource = GetComponent<AudioSource>();
        colliderA = GetComponent<Collider>();
        doorTop.SetActive(true);
        doorBottom.SetActive(true);
        doorDefaultScale = doorTop.transform.localScale;
        playerStartPos.GetComponent<Renderer>().enabled = false;

        StartDoorAsOpen(doorBottom);
    }

    void StartDoorAsOpen(GameObject door)
    {
        door.transform.position += (Vector3.left * 2);
        door.transform.localScale -= (Vector3.one * 0.01f);
    }

    void Update()
    {
        if (playingRoom && spawnedEnemies.Count == 0) //killed all enemies
        {
            ClearedRoom();
        }
    }

    private void ClearedRoom()
    {
        Invoke("PlayRoomClearedSound", 0.2f);
        playingRoom = false;
        //clearedRoom = true;
        EnablePickups();

        AnimateDoorOpen(doorTop);
    }

    private void AnimateDoorOpen(GameObject door)
    {
        LeanTween.move(door, door.transform.position - (Vector3.right * 2), 1f)
            .setDelay(1f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.scale(door, (doorDefaultScale - new Vector3(0.01f, 0, 0.01f)), 1f)
            .setDelay(1f).setEase(LeanTweenType.easeInOutCubic);
    }


    private void PlayRoomEnterSound()
    {
        audioSource.PlayOneShot(audioClips[0]);
    }
    private void PlayRoomClearedSound()
    {
        audioSource.PlayOneShot(audioClips[1]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (unenteredRoom && other.gameObject.CompareTag("Player"))
        {
            //will only trigger once, when the player enters the room area
            BeginRoom();
            colliderA.enabled = false;
        }

    }

    private void BeginRoom()
    {
        //Invoke("EnableEnemies", 0f);
        EnableEnemies(); //only executed once

        unenteredRoom = false;
        playingRoom = true;
        PlayRoomEnterSound();
        AnimateDoorClosed(doorBottom);
    }
    private void AnimateDoorClosed(GameObject door)
    {
        LeanTween.move(door, door.transform.position + (Vector3.right * 2), 0.5f)
            .setEase(LeanTweenType.easeInOutCubic);
        LeanTween.scale(door, doorDefaultScale, 0.5f)
            .setEase(LeanTweenType.easeInOutCubic);
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
                newEnemyInstance.transform.SetParent(this.transform, true);
                newEnemyInstance.gameObject.name = newEnemyInstance.gameObject.name + " " + i;
                spawnedEnemies.Add(newEnemyInstance);
                enemySpawnPoints.RemoveAt(randSpawnPointIndex);
            }
        }
    }
    void EnableEnemies() //executed once
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            spawnedEnemies[i].SetActive(true);
            //OLD ENEMY METHOD spawnedEnemies[i].GetComponent<EnemyBehaviorA>().SpawnEnableEnemy();

            //all child classes derive from EnemyBase!
            spawnedEnemies[i].GetComponent<EnemyBase>().EnableEnemy();
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
                //newPickupInstance.GetComponent<EnemyBehaviorA>().enabled = true;
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
