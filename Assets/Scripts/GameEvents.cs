using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    private void Awake()
    {
        if (instance != null) //if one exists already, delete this instance
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public event Action<string, int, int, float, float, float> upgradePlayerStats; //upgrade player stats
    public event Action upgradeItemPlaySFX; //play SFX when touched


    //UpgradeItemTriggerEnter is called in the UpgradeItemBehavior OnTriggerEnter
    //it has overloads depending on what needs to be passed in

    public void UpgradeItemTriggerEnter(string tag, int price, int health, float damage, float speed, float firerate)
    {
        if (upgradePlayerStats != null)
        {
            //this is the Action that the player has subscribed to
            upgradePlayerStats(tag, price, health, damage, speed, firerate);
        }
    }

    public void UpgradeItemTriggerEnter()
    {
        if (upgradeItemPlaySFX != null)
        {
            //this is the Action that GlobalOnDestroySounds has subscribed to
            upgradeItemPlaySFX();
        }
    }

}