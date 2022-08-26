using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class FloorLayoutManager : MonoBehaviour
{
    [SerializeField] private GameObject startingRoom;
    [SerializeField] private GameObject bossRoom;
    [SerializeField] private GameObject itemRoom;
    [SerializeField] private GameObject dispenserRoom;
    [SerializeField] private GameObject storeRoom;
    [SerializeField] private List<GameObject> possibleHostileRoomsList;
    [SerializeField] private List<GameObject> spawnedRooms;

    [SerializeField] private NavMeshSurface surface;

    private int roomsToSpawn = 15;
    private float roomZSpacing = 11f;
    private Vector3 startingRoomSpawnPos = Vector3.zero;

    #region public get variables
    public int pub_roomsToSpawn
    {
        get { return roomsToSpawn; }
    }
    public float pub_roomZSpacing
    {
        get { return roomZSpacing; }
    }
    public Vector3 pub_startingRoomSpawnPos
    {
        get { return startingRoomSpawnPos; }
    }
    #endregion


    private void Awake()
    {
        SetAllPoolRoomsInactive();
    }

    private void Start()
    {
        SpawnRooms(roomsToSpawn);

        surface.BuildNavMesh();

    }


    //maybe split the floor into thirds? 1/3 in is an item room, 2/3 is the store?
    //have free item room around halfway, store right before boss


    private void SetAllPoolRoomsInactive()
    {
        startingRoom.SetActive(false);
        bossRoom.SetActive(false);
        itemRoom.SetActive(false);
        dispenserRoom.SetActive(false);
        storeRoom.SetActive(false);

        for (int i = 0; i < possibleHostileRoomsList.Count; i++)
        {
            possibleHostileRoomsList[i].SetActive(false);
        }
    }

    private void SpawnRooms(int roomsToSpawn)
    {
        int halfwayPoint = Mathf.CeilToInt(roomsToSpawn/2);
        int oneThirdPoint = Mathf.CeilToInt(roomsToSpawn / 3);
        int twoThirdsPoint = oneThirdPoint * 2;
        //Debug.Log(oneThirdPoint + " and " + twoThirdsPoint);

        for (int i = 0; i < roomsToSpawn; i++)
        {
            if (i == 0) //starting room
            {
                startingRoom.SetActive(true);
                startingRoom.transform.position = Vector3.zero;
            }
            else if (i == oneThirdPoint) //one of two "kind" rooms - item room
            {
                float zSpawnDist = i * roomZSpacing;
                itemRoom.SetActive(true);
                itemRoom.transform.position = new Vector3(0, 0, zSpawnDist);
            }
            else if (i == twoThirdsPoint) //two of two "kind" rooms - dispenser room
            {
                float zSpawnDist = i * roomZSpacing;
                dispenserRoom.SetActive(true);
                dispenserRoom.transform.position = new Vector3(0, 0, zSpawnDist);
            }
            else if (i == roomsToSpawn - 2) //store room right before boss
            {
                float zSpawnDist = i * roomZSpacing;
                storeRoom.SetActive(true);
                storeRoom.transform.position = new Vector3(0, 0, zSpawnDist);
            }
            else if (i == roomsToSpawn - 1) //final room (aka boss)
            {
                float zSpawnDist = i * roomZSpacing;
                bossRoom.SetActive(true);
                bossRoom.transform.position = new Vector3(0, 0, zSpawnDist);
            }
            else //HOSTILE ROOM POOL
            {
                float zSpawnDist = i * roomZSpacing;
                int randRoomIndex = Random.Range(0, possibleHostileRoomsList.Count);
                possibleHostileRoomsList[randRoomIndex].SetActive(true);
                possibleHostileRoomsList[randRoomIndex].transform.position = new Vector3(0, 0, zSpawnDist);
                possibleHostileRoomsList.RemoveAt(randRoomIndex);
            }
        }
    }


}
