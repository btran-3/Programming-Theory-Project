using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class RoomStarting : MonoBehaviour
{
    private int playerId = 0;
    private Player rewiredPlayer;

    private enum InputType { KEYBOARD, JOYSTICK};
    private InputType _inputType;

    private void Start()
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerId);
        _inputType = InputType.KEYBOARD;
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
                    Debug.Log("current input is joystick");
                    _inputType = InputType.JOYSTICK;
                    break;

                case ControllerType.Keyboard:
                default:
                    Debug.Log("current input is keyboard");
                    _inputType = InputType.KEYBOARD;
                    break;
            }
        }
    }


    IEnumerator DisableStartingRoom(float delay)
    {
        yield return new WaitForSeconds(delay);
        //this.gameObject.SetActive(false);
        this.enabled = false;
    }


}
