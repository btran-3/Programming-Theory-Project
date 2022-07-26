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



    public event Action<string, int, float, float, float> onUpgradeItemTriggerEnter;

    //this method is called when Upgrade Item trigger has been entered
    public void UpgradeItemTriggerEnter(string tag, int health, float damage, float speed, float firerate)
    {
        if (onUpgradeItemTriggerEnter != null)
        {
            //this is the Action that the player has subscribed to
            onUpgradeItemTriggerEnter(tag, health, damage, speed, firerate);
        }
    }

}
