using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DispenserBehavior : MonoBehaviour
{
    [SerializeField] GameObject pickupToDispense;
    [SerializeField] TextMeshPro priceTextTMP;
    [SerializeField] int cost;
    [SerializeField] PlayerBehavior playerBehavior;

    public int pub_cost { get { return cost; }}

    private bool isPickupWaitingOnTop;

    private void Start()
    {
        priceTextTMP.SetText("$" + cost + "/each");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerBehavior.pub_currentPlayerMoney >= cost && !isPickupWaitingOnTop)
        {
            DispensePickup();
            GameEvents.instance.DispenserItemCollisionEnter(cost);
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pickup"))
        {
            isPickupWaitingOnTop = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pickup"))
        {
            isPickupWaitingOnTop = false;
        }
    }

    void DispensePickup()
    {
        GameObject newPickup = Instantiate(pickupToDispense,
            transform.position + new Vector3(0, 1.5f, 0), pickupToDispense.transform.localRotation);
        newPickup.SetActive(true);
        newPickup.GetComponent<PickupBehavior>().SpawnPickupAtDispenser();
    }

}
