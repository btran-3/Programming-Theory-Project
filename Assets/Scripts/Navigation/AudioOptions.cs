using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioOptions : MonoBehaviour
{
    public static float soundEffectsVolume { get; private set; }
    public static float musicVolume { get; private set; }

    public static AudioOptions instance;

    [SerializeField] Slider soundEffectsSlider;
    [SerializeField] Slider musicVolumeSlider;

    private static bool hasSoundBeenInitialized;

    private void Awake()
    {
        //use the instance that already exists in the scene
        //this allows retaining references to scene-specific objects

        if (instance == null) //keep this first instance
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) //this instance is not the same as existing one, destroy the old one and use new one
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (!hasSoundBeenInitialized) //set audio defaults once
        {
            hasSoundBeenInitialized = true;
            soundEffectsVolume = 1;
            musicVolume = 1;
        }

        soundEffectsSlider.value = soundEffectsVolume;
        musicVolumeSlider.value = musicVolume;
    }

    public void OnSoundEffectsSliderValueChange(float value)
    {
        soundEffectsVolume = value;
        Debug.Log("SFX " + value);

        //update sound effect mixer volume here
    }

    public void OnMusicSliderValueChange(float value)
    {
        musicVolume = value;
        Debug.Log("Music " + value);

        MusicManager.instance.UpdateMusicMixerVolume(musicVolume);
    }

}
