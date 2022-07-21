using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraCinemachine : MonoBehaviour
{
    [SerializeField] FloorLayoutManager floorLayoutManager;
    [SerializeField] GameObject virtualCamsParent;
    [SerializeField] CinemachineVirtualCamera virtualCameraToInstance;
    [SerializeField] List<CinemachineVirtualCamera> virtualCamerasInstanced;

    private int roomsToSpawn;
    private float roomZSpacing;


    private void Awake()
    {
        //Debug.Log("modify camera script so instances of virtual cameras are generated at runtime");
        roomsToSpawn = floorLayoutManager.pub_roomsToSpawn;
        roomZSpacing = floorLayoutManager.pub_roomZSpacing;

        //Debug.Log(roomsToSpawn);

        for (int i = 0; i < roomsToSpawn; i++)
        {
            CinemachineVirtualCamera cam = Instantiate(virtualCameraToInstance,
                virtualCamsParent.transform.position + (Vector3.forward * i * roomZSpacing),
                virtualCameraToInstance.transform.rotation);
            cam.transform.SetParent(virtualCamsParent.transform, true);
            virtualCamerasInstanced.Add(cam);
        }


        for (int i = 0; i < virtualCamerasInstanced.Count; i++)
        {

            virtualCamerasInstanced[i].gameObject.SetActive(false);

        }
        virtualCamerasInstanced[0].gameObject.SetActive(true);
    }


    public void Start()
    {

    }


    public void ActivateNextCamera(int camIndex)
    {
        virtualCamerasInstanced[camIndex].gameObject.SetActive(true);

        if ((camIndex - 2) >= 0) // is index 0 or more
        {
            if (virtualCamerasInstanced[camIndex - 2].gameObject.activeInHierarchy == true) //is it enabled?
            {
                virtualCamerasInstanced[camIndex - 2].gameObject.SetActive(false);
            }
        }
    }

}
