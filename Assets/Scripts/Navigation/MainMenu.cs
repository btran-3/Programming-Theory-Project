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

    [SerializeField] GameObject newGameButton, optionsButton, exitButton;
    [SerializeField] GameObject optionsSoundEffectsSlider, optionsMusicSlider, optionsSoundEffectText, optionsMusicText;
    GameObject lastSelected;

    private bool isMenuTransitioning;
    private float menuAnimationOffset = 250f;
    private float menuAnimationTime = 0.65f;
    [SerializeField] AnimationCurve menuAnimationCurve;

    private float unselectedUIColorFloat = 0.196f; //323232 divided by 255
    private float selectedUIColorFloat = 0.96f; //F5F5F5 divided by 255

    //Rewired stuff
    private int playerId = 0;
    private Player player;

    private enum MenuState { MAINMENU, OPTIONSMENU};
    private MenuState menuState;


    void Start()
    {
        menuState = MenuState.MAINMENU;

        player = ReInput.players.GetPlayer(playerId);

        InitMenuPanels();

        DisableMenuButtons();

        StartCoroutine(ReenableMainMenuButtons(menuAnimationTime));

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
    }

    void Update()
    {
        if (player.GetButtonDown("UICancel") && menuState == MenuState.OPTIONSMENU)
        {
            CloseOptionsMenu();
        }

        //if hovering on Sound Effects volume text, which covers the slider too
        if (EventSystem.current.currentSelectedGameObject == optionsSoundEffectsSlider && lastSelected != optionsSoundEffectsSlider)
        {
            SelectSoundEffectsSlider();
        }
        //if hovering on Music volume Text, which covers the slider too
        else if (EventSystem.current.currentSelectedGameObject == optionsMusicSlider && lastSelected != optionsMusicSlider)
        {
            SelectMusicSlider();
        }
    }


    #region buttons disable/reenable
    private void DisableMenuButtons()
    {
        newGameButton.GetComponent<Button>().interactable = false;
        //continueButton.GetComponent<Button>().interactable = false;
        optionsButton.GetComponent<Button>().interactable = false;
        exitButton.GetComponent<Button>().interactable = false;
    }
    IEnumerator ReenableMainMenuButtons(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        newGameButton.GetComponent<Button>().interactable = true;
        //continueButton.GetComponent<Button>().interactable = true;
        optionsButton.GetComponent<Button>().interactable = true;
        exitButton.GetComponent<Button>().interactable = true;
    }
    #endregion

    private void InitMenuPanels()
    {
        optionsMenu.transform.position += new Vector3(menuAnimationOffset, 0, 0);

        mainMenu.SetActive(true);
        mainMenu.GetComponent<CanvasGroup>().alpha = 1;
        optionsMenu.GetComponent<CanvasGroup>().alpha = 0;
        optionsMenu.SetActive(false);
    }

    public void OpenOptionsMenu()
    {
        LeanTween.cancel(mainMenu);
        LeanTween.cancel(optionsMenu);

        player.controllers.maps.SetMapsEnabled(false, "Menu Category");
        StartCoroutine(ChangeRewiredInputStatus("Menu Category", true, menuAnimationTime));
        menuState = MenuState.OPTIONSMENU;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);


        StartCoroutine(DisableThisObject(mainMenu, menuAnimationTime));
        LeanTween.moveX(mainMenu, mainMenu.transform.position.x - menuAnimationOffset, menuAnimationTime).setEase(menuAnimationCurve);
        LeanTween.value(1, 0, menuAnimationTime).setOnUpdate(FadeOutMainMenu);

        optionsMenu.SetActive(true);
        LeanTween.moveX(optionsMenu, optionsMenu.transform.position.x - menuAnimationOffset, menuAnimationTime).setEase(menuAnimationCurve);
        LeanTween.value(0, 1, menuAnimationTime).setOnUpdate(FadeInOptionsMenu);
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


        StartCoroutine(DisableThisObject(optionsMenu, menuAnimationTime));
        LeanTween.moveX(mainMenu, mainMenu.transform.position.x + menuAnimationOffset, menuAnimationTime).setEase(menuAnimationCurve);
        LeanTween.value(0, 1, menuAnimationTime).setOnUpdate(FadeInMainMenu);

        mainMenu.SetActive(true);
        LeanTween.moveX(optionsMenu, optionsMenu.transform.position.x + menuAnimationOffset, menuAnimationTime).setEase(menuAnimationCurve);
        LeanTween.value(1, 0, menuAnimationTime).setOnUpdate(FadeOutOptionsMenu);
    }

    private void SelectMusicSlider()
    {
        lastSelected = optionsMusicSlider;

        LeanTween.cancel(optionsSoundEffectText);
        LeanTween.cancel(optionsMusicText);

        LeanTween.value(optionsSoundEffectText.GetComponent<TextMeshProUGUI>().color.r, unselectedUIColorFloat, 0.1f).setOnUpdate(ColorSoundEffectsText);
        LeanTween.value(optionsMusicText.GetComponent<TextMeshProUGUI>().color.r, selectedUIColorFloat, 0.1f).setOnUpdate(ColorMusicText);
    }

    private void SelectSoundEffectsSlider()
    {
        lastSelected = optionsSoundEffectsSlider;

        LeanTween.cancel(optionsSoundEffectText);
        LeanTween.cancel(optionsMusicText);

        LeanTween.value(optionsSoundEffectText.GetComponent<TextMeshProUGUI>().color.r, selectedUIColorFloat, 0.1f).setOnUpdate(ColorSoundEffectsText);
        LeanTween.value(optionsMusicText.GetComponent<TextMeshProUGUI>().color.r, unselectedUIColorFloat, 0.1f).setOnUpdate(ColorMusicText);
    }

    #region Rewired

    IEnumerator ChangeRewiredInputStatus(string categoryName, bool state, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        player.controllers.maps.SetMapsEnabled(state, categoryName);
    }
    #endregion

    #region menu fade and color animations for LeanTween SetOnUpdate
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

    #endregion

    IEnumerator DisableThisObject(GameObject disableThis, float delay)
    {
        //disables main or options menu after animating off
        yield return new WaitForSecondsRealtime(delay);
        disableThis.SetActive(false);
    }

    public void SelectThisGameobject(GameObject select)
    {
        //this is used for hovering over non-button text to select their respective sliders
        EventSystem.current.SetSelectedGameObject(select);
    }

}
