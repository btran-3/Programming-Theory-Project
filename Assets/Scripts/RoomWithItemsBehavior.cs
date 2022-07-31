using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomWithItemsBehavior : MonoBehaviour
{
    [SerializeField] List<GameObject> possibleUpgradeItems;
    [SerializeField] List<GameObject> itemsSpawnPoints;

    [SerializeField] List<GameObject> spawnedUpgradeItems;

    private Vector3 doorDefaultScale;
    [SerializeField] GameObject doorTop;
    [SerializeField] GameObject doorBottom;
    [Space(10)]
    [SerializeField] GameObject playerStartPos;
    [Space(10)]

    private float mainLightDefaultIntensity;
    private float spotLightMaxIntensity;
    [SerializeField] Light mainLight;
    [SerializeField] List<Light> spotLights;

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
        doorBottom.SetActive(false);
        colliderA = GetComponent<Collider>();
        playerStartPos.GetComponent<Renderer>().enabled = false;

        InitializeUpgradeItems();

        doorDefaultScale = doorTop.transform.localScale;
        mainLightDefaultIntensity = mainLight.intensity;
        spotLightMaxIntensity = 300f;
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
        LeanTween.value(gameObject, mainLightDefaultIntensity, 0.1f, 1f).setEase(LeanTweenType.easeInOutSine).setOnUpdate(ChangeMainLightIntensity);
        LeanTween.value(gameObject, 0, spotLightMaxIntensity, 1f).setEase(LeanTweenType.easeInOutSine).setOnUpdate(ChangeSpotLightIntensity);

        for (int i = 0; i < spawnedUpgradeItems.Count; i++)
        {
            spawnedUpgradeItems[i].SetActive(true);
        }
    }

    void ChangeMainLightIntensity(float value)
    {
        mainLight.intensity = value;
    }

    void ChangeSpotLightIntensity(float value)
    {
        spotLights[0].intensity = value;
        spotLights[1].intensity = value;
    }


    public void PlayerTookAnItem(GameObject takenItem)
    {
        didPlayerTakeAnItem = true;
        spawnedUpgradeItems.Remove(takenItem);

        for (int i = 0; i < spawnedUpgradeItems.Count; i++)
        {
            spawnedUpgradeItems[i].GetComponent<Collider>().enabled = false;
            //LeanTween.scale(spawnedUpgradeItems[i], Vector3.zero, 0.3f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(DestroySpawnedItemsInList);
            LeanTween.moveLocalY(spawnedUpgradeItems[i], 65f, 1.25f).setEase(LeanTweenType.easeInCubic).setOnComplete(DestroySpawnedItemsInList);
            LeanTween.moveLocalZ(spawnedUpgradeItems[i], 25f, 0.35f).setEase(LeanTweenType.easeInSine).setDelay(0.9f);
        }
        AnimateDoorOpen(doorTop);

        LeanTween.value(gameObject, 0.1f, mainLightDefaultIntensity, 1f).setEase(LeanTweenType.easeInOutSine).setOnUpdate(ChangeMainLightIntensity);
        LeanTween.value(gameObject, spotLightMaxIntensity, 0f, 1f).setEase(LeanTweenType.easeInOutSine).setOnUpdate(ChangeSpotLightIntensity);
    }


    private void AnimateDoorOpen(GameObject door)
    {
        LeanTween.move(door, door.transform.position - (Vector3.right * 2), 1f)
            .setDelay(0.5f).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.scale(door, (doorDefaultScale - new Vector3(0.01f, 0, 0.01f)), 1f)
            .setDelay(0.5f).setEase(LeanTweenType.easeInOutCubic);
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

    void DestroySpawnedItemsInList()
    {
        for (int i = 0; i < spawnedUpgradeItems.Count; i++)
        {
            GameObject instance = spawnedUpgradeItems[i];
            spawnedUpgradeItems.Remove(instance);
            Destroy(instance);
        }
        
    }

}
