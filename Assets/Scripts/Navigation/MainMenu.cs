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

    //Rewired stuff
    private int playerId = 0;
    private Player player;

    private enum ActiveMenu { MAINMENUACTIVE, OPTIONSMENUACTIVE };
    private ActiveMenu activeMenu;
    private enum MainMenuState { NEWGAME, CONTINUE, OPTIONS, EXIT };
    private MainMenuState mainMenuState;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerId);

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);

        optionsMenu.SetActive(true);

        activeMenu = ActiveMenu.MAINMENUACTIVE;
        mainMenuState = MainMenuState.NEWGAME;
    }

    // Update is called once per frame
    void Update()
    {
        switch (mainMenuState)
        {
            case MainMenuState.NEWGAME:


                break;
            case MainMenuState.CONTINUE:
                break;
            case MainMenuState.OPTIONS:
                break;
            case MainMenuState.EXIT:
                break;
            default:
                break;
        }

        if (EventSystem.current == null) //if nothing is selected
        {
            EventSystem.current.SetSelectedGameObject(optionsFirstButton);
        }

        if (player.GetButtonDown("Cancel Selection") && optionsMenu.activeInHierarchy)
        {
            CloseOptionsMenu();
            Debug.Log("successfully closing menu using Cancel Selection button");
        }
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    private void CloseOptionsMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(optionsClosedButton);

        optionsMenu.SetActive(false);


    }
}
