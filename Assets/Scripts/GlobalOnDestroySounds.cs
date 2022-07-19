using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalOnDestroySounds : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] playerDeathSounds;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void playPlayerDeathSound()
    {
        int rand = Random.Range(0, playerDeathSounds.Length);
        audioSource.PlayOneShot(playerDeathSounds[rand]);
    }

}
