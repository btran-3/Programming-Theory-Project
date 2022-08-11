using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeItemBehavior : MonoBehaviour
{
    [SerializeField] RoomWithItemsBehavior roomWithItemsBehavior;

    [SerializeField] bool isItemFree;
    [SerializeField] TextMeshPro priceTextTMP;
    [SerializeField] int price;

    #region upgrade stats
    [SerializeField] string itemTag;
    [SerializeField] int healthUpAmt;
    [SerializeField] float damageUpAmt;
    [SerializeField] float speedUpAmt;
    [SerializeField] float firerateUpAmt;
    #endregion

    #region public get set
    public int pub_price
    {
        get { return price; }
    }

    public int pub_healthUpAmt
    {
        get { return healthUpAmt; }
    }

    public float pub_damageUpAmt
    {
        get { return damageUpAmt; }
    }

    public float pub_speedUpAmt
    {
        get { return speedUpAmt; }
    }

    public float pub_firerateUp
    {
        get { return firerateUpAmt; }
    }

    public bool pub_isItemFree
    {
        get { return isItemFree; }
    }
    #endregion

    [SerializeField] PlayerBehavior playerBehavior;
    [SerializeField] GameObject ondestroySounds;

    //private bool hasBeenTouched;
    Collider itemCollider;

    private void Awake()
    {
        //gameObject.SetActive(false);
        itemCollider = GetComponent<Collider>();
        priceTextTMP.enabled = false;
    }

    void Start()
    {
        SetPriceTagText();
        //gameObject.SetActive(false);
    }

    private void SetPriceTagText()
    {
        if (!isItemFree) //if not free, it has a cost
        {
            if (price == 0)
            {
                Debug.LogWarning("Item that should have a cost is not assigned one");
            }
            else
            {
                priceTextTMP.SetText("$" + price);
                priceTextTMP.enabled = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CheckIfPlayerCanTakeItem();
        }
    }

    private void CheckIfPlayerCanTakeItem()
    {
        if (playerBehavior.pub_currentPlayerMoney >= price && !isItemFree) //if player can afford and item costs money
        {
            itemCollider.enabled = false;
            //upgrade player (PlayerBehavior)
            GameEvents.instance.UpgradeItemTriggerEnter(itemTag, price, healthUpAmt, damageUpAmt, speedUpAmt, firerateUpAmt);
            //play SFX (GlobalOnDestroySounds)
            GameEvents.instance.UpgradeItemTriggerEnter();

            LeanTween.scale(gameObject, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(DestroyObject);
        }
        else if (isItemFree) //if the item is free
        {
            itemCollider.enabled = false;
            //upgrade player (PlayerBehavior) and DO NOT pass a price amount
            GameEvents.instance.UpgradeItemTriggerEnter(itemTag, 0, healthUpAmt, damageUpAmt, speedUpAmt, firerateUpAmt);
            //play SFX (GlobalOnDestroySounds)
            GameEvents.instance.UpgradeItemTriggerEnter();

            LeanTween.scale(gameObject, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(DestroyObject);

            if (roomWithItemsBehavior != null) //make the other free item disappear
            {
                roomWithItemsBehavior.PlayerTookAnItem(gameObject);
            }
        }
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        
    }
}
