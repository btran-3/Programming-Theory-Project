using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItemBehavior : MonoBehaviour
{
    [SerializeField] bool isItemFree;
    [SerializeField] GameObject priceText;

    #region upgrades
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

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
