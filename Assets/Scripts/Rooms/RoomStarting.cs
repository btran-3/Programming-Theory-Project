using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class RoomStarting : MonoBehaviour
{
    [SerializeField] GameObject keyboardControlsPanel;
    [SerializeField] GameObject joystickControlsPanel;

    private int playerId = 0;
    private Player rewiredPlayer;

    //private enum InputType { KEYBOARD, JOYSTICK};
    //private InputType _inputType;

    private void Start()
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerId);
        //_inputType = InputType.KEYBOARD;
        keyboardControlsPanel.SetActive(true);

        GameEvents.instance.playerEnteredNewRoom += DisableStartingRoom;
    }

    private void Update()
    {
        // Get last controller from a Player and the determine the type of controller being used
        Controller controller = rewiredPlayer.controllers.GetLastActiveController();
        if (controller != null)
        {
            switch (controller.type)
            {
                case ControllerType.Joystick:
                    //_inputType = InputType.JOYSTICK;

                    joystickControlsPanel.SetActive(true);
                    keyboardControlsPanel.SetActive(false);
                    break;

                case ControllerType.Keyboard:
                default:
                    //_inputType = InputType.KEYBOARD;

                    joystickControlsPanel.SetActive(false);
                    keyboardControlsPanel.SetActive(true);
                    break;
            }
        }
    }

    void DisableStartingRoom()
    {
        StartCoroutine(DisableStartingRoomDelay(2f));
    }

    IEnumerator DisableStartingRoomDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //this.gameObject.SetActive(false);
        this.enabled = false;
    }

    private void OnDestroy()
    {
        GameEvents.instance.playerEnteredNewRoom -= DisableStartingRoom;
    }

}
