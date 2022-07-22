using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalOnDestroySounds : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] playerDeathSounds;
    [SerializeField] private AudioClip projectileHitObstacleSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void playPlayerDeathSound()
    {
        int rand = Random.Range(0, playerDeathSounds.Length);
        audioSource.PlayOneShot(playerDeathSounds[rand]);
    }

    public void playProjectileHitObstacleSound()
    {
        audioSource.PlayOneShot(projectileHitObstacleSound, 0.3f);
    }

}
