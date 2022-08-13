using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackground : MonoBehaviour
{
    [SerializeField] Light spotLight;
    
    void Start()
    {
        LeanTween.rotateAround(gameObject, Vector3.up, -360, 16f).setLoopClamp();
        LeanTween.value(30f, 40f, 1.5f).setEaseInOutSine().setOnUpdate(SpotlightPulseBrightness).setLoopPingPong();
        LeanTween.value(75f, 110f, 3f).setEaseInOutSine().setOnUpdate(SpotLightOuterAngle).setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpotlightPulseBrightness(float value)
    {
        spotLight.intensity = value;
    }
    void SpotLightOuterAngle(float value)
    {
        spotLight.spotAngle = value;
        //Debug.Log(spotLight.spotAngle);
    }

    private void OnDestroy()
    {
        LeanTween.cancelAll();
    }

}
