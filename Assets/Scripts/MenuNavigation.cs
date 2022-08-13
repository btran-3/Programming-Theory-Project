using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(500)]
public class MenuNavigation : MonoBehaviour
{
    public static MenuNavigation instance;
    [SerializeField] Image blackFade;

    private int playerId = 0;
    private Player rewiredPlayer;

    private void Awake()
    {
        //this allows retaining references to the original instances in each scene rather than destroying originals

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

        rewiredPlayer = ReInput.players.GetPlayer(playerId);

    }

    private void Start()
    {
        Time.timeScale = 1;

        blackFade.gameObject.SetActive(true);
        LeanTween.value(1, 0, 1f).setDelay(1f).setIgnoreTimeScale(true).setOnUpdate(UpdateBlackFadeAlpha);


        rewiredPlayer.controllers.maps.SetAllMapsEnabled(false); //disable all maps by default

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 0) //menu scene
        {
            MusicManager.instance.SwapTrack(MusicManager.instance.pub_defaultAmbiance);
            StartCoroutine(ChangeRewiredInputStatus("Menu Category", true, 1f)); //prevent running into existing fading animation
        }
        else if (sceneIndex == 1) //main game
        {
            MusicManager.instance.SwapTrack(MusicManager.instance.pub_hostileMusic);
            StartCoroutine(ChangeRewiredInputStatus("Default", true, 2f)); //prevent running into existing fading animation
        }

        /*
        foreach (ControllerMap map in rewiredPlayer.controllers.maps.GetAllMapsInCategory("Default"))
        {
            Debug.Log(map);
        } */

    }

    public void LoadThisScene(int sceneIndex)
    {
        if (sceneIndex == 0) //go to main menu
        {
            FadeToBlack();
            StartCoroutine(DelaySwitchScene(sceneIndex, 1));
        }
        else if (sceneIndex == 1) //go to Main game
        {
            FadeToBlack();
            StartCoroutine(DelaySwitchScene(sceneIndex, 1));
        }
    }

    void FadeToBlack()
    {
        blackFade.gameObject.SetActive(true);
        LeanTween.cancel(blackFade.gameObject);
        blackFade.color = new Color(0, 0, 0, 0); //0 opacity

        LeanTween.value(0, 1, 1f).setIgnoreTimeScale(true).setOnUpdate(UpdateBlackFadeAlpha);
    }

    void FadeFromBlack()
    {
        blackFade.gameObject.SetActive(true);
        LeanTween.cancel(blackFade.gameObject);
        blackFade.color = new Color(0, 0, 0, 1); //100% opacity

        LeanTween.value(1, 0, 1f).setIgnoreTimeScale(true).setOnUpdate(UpdateBlackFadeAlpha);
        //Debug.Log("Should start fading out now");
    }

    IEnumerator DelaySwitchScene(int sceneIndex, float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        FadeFromBlack();
        SceneManager.LoadScene(sceneIndex);
    }

    void UpdateBlackFadeAlpha(float alphaChange)
    {
        if (blackFade != null)
        {
            blackFade.color = new Color(0, 0, 0, alphaChange);
            //Debug.Log("fading to " + alphaChange + " now");
            if (alphaChange == 0)
            {
                blackFade.gameObject.SetActive(false);
            }
        }
        else
        {
            //Debug.LogWarning("backFade doesn't exist or something");
        }

    }

    IEnumerator ChangeRewiredInputStatus(string categoryName, bool state, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        rewiredPlayer.controllers.maps.SetMapsEnabled(state, categoryName);
    }

}
