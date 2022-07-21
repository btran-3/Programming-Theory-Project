using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class FloorLayoutManager : MonoBehaviour
{
    [SerializeField] private GameObject startingRoom;
    [SerializeField] private GameObject endingRoom;
    [SerializeField] private GameObject[] possibleHostileRooms;
    [SerializeField] private GameObject[] possibleKindRooms;
    [SerializeField] private List<GameObject> spawnedRooms;

    [SerializeField] private NavMeshSurface surface;

    private int roomsToSpawn = 9;
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


    private void SetAllPoolRoomsInactive()
    {
        startingRoom.SetActive(false);
        endingRoom.SetActive(false);

        for (int i = 0; i < possibleHostileRooms.Length; i++)
        {
            possibleHostileRooms[i].SetActive(false);
        }
        for (int i = 0; i < possibleKindRooms.Length; i++)
        {
            possibleKindRooms[i].SetActive(false);
        }
    }

    private void SpawnRooms(int roomsToSpawn)
    {
        int halfwayPoint = Mathf.CeilToInt(roomsToSpawn/2);
        //Debug.Log(halfwayPoint);

        for (int i = 0; i < roomsToSpawn; i++)
        {
            if (i == 0) //starting room
            {
                GameObject room = Instantiate(startingRoom, startingRoomSpawnPos, Quaternion.identity);
                room.SetActive(true);
            }
            else if (i == halfwayPoint) //a "kind" room roughly halfway through
            {
                float zSpawnDist = i * roomZSpacing;
                GameObject room = Instantiate(possibleKindRooms[0],
                    startingRoomSpawnPos + new Vector3(0, 0, zSpawnDist), Quaternion.identity);
                room.SetActive(true);
            }
            else if (i == roomsToSpawn) //THIS IS SUPPOSED TO BE THE ENDING ROOM
            {
                float zSpawnDist = i * roomZSpacing;
                GameObject room = Instantiate(endingRoom,
                    startingRoomSpawnPos + new Vector3(0, 0, zSpawnDist), Quaternion.identity);
                room.SetActive(true);
            }
            else //HOSTILE ROOM POOL
            {
                float zSpawnDist = i * roomZSpacing;
                int randRoomIndex = Random.Range(0, possibleHostileRooms.Length);
                GameObject room = Instantiate(possibleHostileRooms[randRoomIndex],
                    startingRoomSpawnPos + new Vector3(0, 0, zSpawnDist), Quaternion.identity);
                room.SetActive(true);
            }
        }
    }


}
