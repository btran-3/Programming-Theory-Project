using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraCinemachine : MonoBehaviour
{

    [SerializeField] CinemachineVirtualCamera[] virtualCameras;

    private void Awake()
    {
        for (int i = 0; i < virtualCameras.Length; i++)
        {

            virtualCameras[i].gameObject.SetActive(false);

        }
        //virtualCameras[0].gameObject.SetActive(true);
    }

    public void ActivateNextCamera(int camIndex)
    {
        virtualCameras[camIndex].gameObject.SetActive(true);

        if ((camIndex - 2) >= 0) // is index 0 or more
        {
            if (virtualCameras[camIndex - 2].gameObject.activeInHierarchy == true) //is it enabled?
            {
                virtualCameras[camIndex - 2].gameObject.SetActive(false);
            }
        }
    }

}
