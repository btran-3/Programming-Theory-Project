using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomShopBehavior : MonoBehaviour
{
    [SerializeField] GameObject[] itemsAndDispensers;

    private Vector3 doorDefaultScale;
    [SerializeField] GameObject doorTop;
    [SerializeField] GameObject doorBottom;
    [Space(10)]
    [SerializeField] GameObject playerStartPos;
    [Space(10)]

    private Collider colliderA;

    public Vector3 pub_playerStartPos
    {
        get { return playerStartPos.transform.position; }
    }


    private void Start()
    {
        doorTop.SetActive(false);
        doorBottom.SetActive(false);
        colliderA = GetComponent<Collider>();
        playerStartPos.GetComponent<Renderer>().enabled = false;

        InitializeUpgradeItems();

        doorDefaultScale = doorTop.transform.localScale;
        StartDoorAsOpen(doorBottom);
    }

    private void Update()
    {

    }

    void StartDoorAsOpen(GameObject door)
    {
        door.transform.position += (Vector3.left * 2);
        door.transform.localScale -= (Vector3.one * 0.01f);
    }

    void InitializeUpgradeItems()
    {
        foreach (var item in itemsAndDispensers)
        {
            item.gameObject.SetActive(true);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            colliderA.enabled = false;
            AnimateDoorClosed(doorBottom);
        }
    }
    private void AnimateDoorClosed(GameObject door)
    {
        door.SetActive(true);
        LeanTween.move(door, door.transform.position + (Vector3.right * 2), 0.5f)
            .setEase(LeanTweenType.easeInOutCubic);
        LeanTween.scale(door, doorDefaultScale, 0.5f)
            .setEase(LeanTweenType.easeInOutCubic);
    }
}
