using UnityEngine;

public class Car : MonoBehaviour
{
    public bool isLightsOn = false;

    public Light[] lights;

    public LightManager lightM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightM = GameManager.Instance.GetComponent<LightManager>();

        SwitchLights(isLightsOn);
    }

    // Update is called once per frame
    void Update()
    {
        if (lightM.isDay != !isLightsOn)
        {
            SwitchLights(!lightM.isDay);
        }
    }

    void SwitchLights(bool on) { 
        isLightsOn = on;

        foreach (Light light in lights)
        {
            light.enabled = on;
        }
    }
}
