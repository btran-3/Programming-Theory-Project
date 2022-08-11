using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalOnDestroySounds : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] pickupCollectedSounds;
    [SerializeField] private AudioClip[] playerDeathSounds;
    [SerializeField] private AudioClip[] enemyDeathSounds;
    [SerializeField] private AudioClip projectileHitObstacleSound;
    [SerializeField] private AudioClip upgradeItemSound;
    //[SerializeField] private AudioClip[] blankPickupSounds;
    [SerializeField] private AudioClip halfHeartPickupSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameEvents.instance.upgradeItemPlaySFX += PlayUpgradeItemSound;
    }

    public void PlayPlayerDeathSound()
    {
        int rand = Random.Range(0, playerDeathSounds.Length);
        audioSource.PlayOneShot(playerDeathSounds[rand]);
    }

    public void PlayEnemyDeathSound(string enemyType)
    {
        switch (enemyType)
        {
            case "normal":
                audioSource.PlayOneShot(enemyDeathSounds[0]);
                break;
            case "normalBigger":
                audioSource.PlayOneShot(enemyDeathSounds[1]);
                break;
            case "flying":
            audioSource.PlayOneShot(enemyDeathSounds[2]);
                break;
            case "shooting":
            audioSource.PlayOneShot(enemyDeathSounds[3]);
                break;
            case "boss":
            audioSource.PlayOneShot(enemyDeathSounds[4]);
                break;
            default:
                Debug.LogWarning("enemy string passed not valid");
                break;

        }
    }

    public void PlayProjectileHitObstacleSound()
    {
        audioSource.PlayOneShot(projectileHitObstacleSound, 0.3f);
    }

    void PlayUpgradeItemSound()
    {
        audioSource.PlayOneShot(upgradeItemSound);
    }

    public void PlayPickupCollectedSound(string pickUpType)
    {
        if (pickUpType == "Blank")
        {
            int rand = Random.Range(0, 3);
            audioSource.PlayOneShot(pickupCollectedSounds[rand]);
        }
        else if (pickUpType == "Money")
        {
            int rand = Random.Range(3, 6);
            audioSource.PlayOneShot(pickupCollectedSounds[rand]);
        }
        else if (pickUpType == "HalfHeart")
        {
            audioSource.PlayOneShot(pickupCollectedSounds[6]);
        }
    }

    private void OnDestroy()
    {
        GameEvents.instance.upgradeItemPlaySFX -= PlayUpgradeItemSound;
    }
}
