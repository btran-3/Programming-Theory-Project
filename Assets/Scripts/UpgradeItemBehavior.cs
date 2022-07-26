using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeItemBehavior : MonoBehaviour
{
    [SerializeField] bool isItemFree;
    [SerializeField] TextMeshPro priceTextTMP;
    [SerializeField] int costNumber;

    #region upgrade stats
    [SerializeField] string itemTag;
    [SerializeField] int healthUpAmt;
    [SerializeField] float damageUpAmt;
    [SerializeField] float speedUpAmt;
    [SerializeField] float firerateUpAmt;
    #endregion

    #region public get set
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
    #endregion

    [SerializeField] GameObject ondestroySounds;
    private bool hasBeenTouched;

    private void Awake()
    {
        //gameObject.SetActive(false);
        priceTextTMP.enabled = false;
    }

    void Start()
    {
        SetPriceTagText();
    }
    private void SetPriceTagText()
    {
        if (!isItemFree) //if not free, it has a cost
        {
            if (costNumber == 0)
            {
                Debug.LogWarning("Item that should have a cost is not assigned one");
            }
            else
            {
                priceTextTMP.SetText("$" + costNumber);
                priceTextTMP.enabled = true;
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!hasBeenTouched && other.gameObject.CompareTag("Player"))
        {
            GameEvents.instance.UpgradeItemTriggerEnter(itemTag, healthUpAmt, damageUpAmt, speedUpAmt, firerateUpAmt);
            LeanTween.scale(gameObject, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(DestroyObject);
            
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
