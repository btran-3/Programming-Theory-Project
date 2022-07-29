using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserBehavior : MonoBehaviour
{
    [SerializeField] GameObject pickupToDispense;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DispensePickup();
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
