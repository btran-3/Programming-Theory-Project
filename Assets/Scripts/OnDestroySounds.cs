using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroySounds : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] AudioClip[] pickUpCollectedSounds;
    [SerializeField] AudioClip[] enemyDeathSounds;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPickupCollectedSound(string pickUpType)
    {
        if (pickUpType == "Blank")
        {
            //it's definitely reading the pickUpType string being passed in.
            int rand = Random.Range(0, 3); //can't be the int.
            //Debug.Log(pickUpCollectedSounds[rand].name); //it's successfully picking out an audioclip!
            audioSource.PlayOneShot(pickUpCollectedSounds[rand]); //is it something in the array?

        }
        else if (pickUpType == "Money")
        {
            int rand = Random.Range(3, 6);
            //Debug.Log(rand);
            audioSource.PlayOneShot(pickUpCollectedSounds[rand]);
        }
        
    }

    public void PlayEnemyDeathSound(string enemyType)
    {
        if (enemyType == "small")
        {
            audioSource.PlayOneShot(enemyDeathSounds[0], 0.7f);
        }
        else if (enemyType == "large")
        {
            int rand = Random.Range(3, 5);
            audioSource.PlayOneShot(enemyDeathSounds[1], 0.7f);
        }
    }

}