using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu, optionsMenu;
    [SerializeField] GameObject mainMenuFirstButton, optionsFirstButton, optionsClosedButton;

    [SerializeField] GameObject newGameButton, continueButton, optionsButton, exitButton;
    [SerializeField] GameObject optionsSoundEffectsSlider, optionsMusicSlider, optionsSoundEffectText, optionsMusicText;
    GameObject lastSelected;

    private bool isMenuTransitioning;
    private float menuAnimationOffset = 250f;
    private float menuAnimationTime = 0.65f;

    //private Color unselectedUIColor = new Color(0.196f, 0.196f, 0.196f, 1); //323232
    //private Color selectedUIColor = new Color(0.96f, 0.96f, 0.96f, 1); //F5F5F5
    private float unselectedUIColorFloat = 0.196f; //323232 divided by 255
    private float selectedUIColorFloat = 0.96f; //F5F5F5 divided by 255

    //Rewired stuff
    private int playerId = 0;
    private Player player;

    private enum MenuState { MAINMENU, OPTIONSMENU};
    private MenuState menuState;


    // Start is called before the first frame update
    void Start()
    {
        menuState = MenuState.MAINMENU;

        player = ReInput.players.GetPlayer(playerId);

        optionsMenu.transform.position += new Vector3(menuAnimationOffset, 0, 0);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);


        mainMenu.SetActive(true);
        mainMenu.GetComponent<CanvasGroup>().alpha = 1;
        optionsMenu.GetComponent<CanvasGroup>().alpha = 0;
        optionsMenu.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetButtonDown("UICancel") && menuState == MenuState.OPTIONSMENU)
        {
            CloseOptionsMenu();
            Debug.Log("successfully closing menu using Cancel Selection button");
        }

        //Debug.Log(EventSystem.current);

        if (EventSystem.current.currentSelectedGameObject == optionsSoundEffectsSlider && lastSelected != optionsSoundEffectsSlider)
        {
            lastSelected = optionsSoundEffectsSlider;
            Debug.Log("options SFX slider has been selected");

            LeanTween.cancel(optionsSoundEffectText);
            LeanTween.cancel(optionsMusicText);
            LeanTween.value(optionsSoundEffectText.GetComponent<TextMeshProUGUI>().color.r, selectedUIColorFloat, 0.1f).setOnUpdate(ColorSoundEffectsText);
            LeanTween.value(optionsMusicText.GetComponent<TextMeshProUGUI>().color.r, unselectedUIColorFloat, 0.1f).setOnUpdate(ColorMusicText);
        }
        else if (EventSystem.current.currentSelectedGameObject == optionsMusicSlider && lastSelected != optionsMusicSlider)
        {
            lastSelected = optionsMusicSlider;
            Debug.Log("options music slider has been selected");

            LeanTween.cancel(optionsSoundEffectText);
            LeanTween.cancel(optionsMusicText);
            LeanTween.cancel(optionsSoundEffectText);
            LeanTween.cancel(optionsMusicText);
            LeanTween.value(optionsSoundEffectText.GetComponent<TextMeshProUGUI>().color.r, unselectedUIColorFloat, 0.1f).setOnUpdate(ColorSoundEffectsText);
            LeanTween.value(optionsMusicText.GetComponent<TextMeshProUGUI>().color.r, selectedUIColorFloat, 0.1f).setOnUpdate(ColorMusicText);
        }
    }

    public void OpenOptionsMenu()
    {
        LeanTween.cancel(mainMenu);
        LeanTween.cancel(optionsMenu);

        player.controllers.maps.SetMapsEnabled(false, "Menu Category");
        StartCoroutine(ChangeRewiredInputStatus("Menu Category", true, menuAnimationTime));
        menuState = MenuState.OPTIONSMENU;

        StartCoroutine(DisableThisObject(mainMenu, menuAnimationTime));
        LeanTween.moveX(mainMenu, mainMenu.transform.position.x - menuAnimationOffset, menuAnimationTime).setEaseInOutCubic();
        LeanTween.value(1, 0, menuAnimationTime).setOnUpdate(FadeOutMainMenu);

        optionsMenu.SetActive(true);
        LeanTween.moveX(optionsMenu, optionsMenu.transform.position.x - menuAnimationOffset, menuAnimationTime).setEaseInOutCubic();
        LeanTween.value(0, 1, menuAnimationTime).setOnUpdate(FadeInOptionsMenu);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);


    }

    private void CloseOptionsMenu()
    {
        LeanTween.cancel(mainMenu);
        LeanTween.cancel(optionsMenu);

        player.controllers.maps.SetMapsEnabled(false, "Menu Category");
        StartCoroutine(ChangeRewiredInputStatus("Menu Category", true, menuAnimationTime));
        menuState = MenuState.MAINMENU;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);

        //LeanTween.moveX(mainMenu, mainMenu.transform.position.x + menuAnimationOffset, menuAnimationTime).setEaseInOutCubic();
        //LeanTween.moveX(optionsMenu, optionsMenu.transform.position.x + menuAnimationOffset, menuAnimationTime).setEaseInOutCubic();



        StartCoroutine(DisableThisObject(optionsMenu, menuAnimationTime));
        LeanTween.moveX(mainMenu, mainMenu.transform.position.x + menuAnimationOffset, menuAnimationTime).setEaseInOutCubic();
        LeanTween.value(0, 1, menuAnimationTime).setOnUpdate(FadeInMainMenu);

        mainMenu.SetActive(true);
        LeanTween.moveX(optionsMenu, optionsMenu.transform.position.x + menuAnimationOffset, menuAnimationTime).setEaseInOutCubic();
        LeanTween.value(1, 0, menuAnimationTime).setOnUpdate(FadeOutOptionsMenu);


        //optionsMenu.SetActive(false);
    }

    IEnumerator ChangeRewiredInputStatus(string categoryName, bool state, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        player.controllers.maps.SetMapsEnabled(state, categoryName);
    }

    IEnumerator DisableThisObject(GameObject disableThis, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        disableThis.SetActive(false);
    }

    void FadeOutMainMenu(float value)
    {
        mainMenu.GetComponent<CanvasGroup>().alpha = value;
    }
    void FadeInOptionsMenu(float value)
    {
        optionsMenu.GetComponent<CanvasGroup>().alpha = value;
    }
    void FadeInMainMenu(float value)
    {
        mainMenu.GetComponent<CanvasGroup>().alpha = value;
    }
    void FadeOutOptionsMenu(float value)
    {
        optionsMenu.GetComponent<CanvasGroup>().alpha = value;
    }

    void ColorSoundEffectsText(float value)
    {
        optionsSoundEffectText.GetComponent<TextMeshProUGUI>().color = new Color(value, value, value, 1);
    }
    void ColorMusicText(float value)
    {
        optionsMusicText.GetComponent<TextMeshProUGUI>().color = new Color(value, value, value, 1);
    }

}
