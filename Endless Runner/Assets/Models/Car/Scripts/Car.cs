using UnityEngine;

public class Car : MonoBehaviour
{
    public bool isLightsOn = false;

    public Light[] lights;

    public LightManager lightM;

    public CarController controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<CarController>();
        }

        if (GameManager.Instance != null)
        {
            lightM = GameManager.Instance.GetComponent<LightManager>();
        }
        else
        {
            isLightsOn = true;
        }

        SwitchLights(isLightsOn);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLight();
    }

    void UpdateLight() {
        if (!lightM) {
            return;
        }

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
