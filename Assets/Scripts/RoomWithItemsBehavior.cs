using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomWithItemsBehavior : MonoBehaviour
{
    [SerializeField] List<GameObject> possibleUpgradeItems;
    [SerializeField] List<GameObject> itemsSpawnPoints;

    [SerializeField] List<GameObject> spawnedUpgradeItems;

    [SerializeField] GameObject doorBottom;
    [SerializeField] GameObject playerStartPos;

    private Collider colliderA;

    //private bool unenteredRoom = true;
    private bool waitingForPlayerToPickAnItem;
    private bool didPlayerTakeAnItem;

    public Vector3 pub_playerStartPos
    {
        get { return playerStartPos.transform.position; }
    }

    public bool pub_didPlayerTakeAnItem
    {
        get { return didPlayerTakeAnItem; }
        private set { didPlayerTakeAnItem = value; }
    }

    private void Start()
    {
        colliderA = GetComponent<Collider>();
        playerStartPos.GetComponent<Renderer>().enabled = false;

        InitializeUpgradeItems();
    }

    private void Update()
    {
        if (waitingForPlayerToPickAnItem)
        {
            int numberOfSpawnedItems = spawnedUpgradeItems.Count;

            if (spawnedUpgradeItems.Count < numberOfSpawnedItems)
            {

            }
        }
    }

    void InitializeUpgradeItems()
    {
        for (int i = 0; i < itemsSpawnPoints.Count; i++)
        {
            int rand = Random.Range(0, possibleUpgradeItems.Count);
            possibleUpgradeItems[rand].transform.position = itemsSpawnPoints[i].transform.position;

            spawnedUpgradeItems.Add(possibleUpgradeItems[rand]);
            possibleUpgradeItems.RemoveAt(rand);
        }
    }

    void EnableUpgradeItems() //executed once
    {
        for (int i = 0; i < spawnedUpgradeItems.Count; i++)
        {
            spawnedUpgradeItems[i].SetActive(true);
        }
    }

    public void PlayerTookAnItem(GameObject takenItem)
    {
        didPlayerTakeAnItem = true;
        spawnedUpgradeItems.Remove(takenItem);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            colliderA.enabled = false;
            EnableUpgradeItems();
            waitingForPlayerToPickAnItem = true;
        }
    }

}
