using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Rewired;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu, optionsMenu;
    [SerializeField] GameObject mainMenuFirstButton, optionsFirstButton, optionsClosedButton;

    [SerializeField] GameObject newGameButton, continueButton, optionsButton, exitButton;

    private bool isMenuTransitioning;
    private float menuAnimationOffset = 250f;
    private float menuAnimationTime = 0.65f;

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

}
