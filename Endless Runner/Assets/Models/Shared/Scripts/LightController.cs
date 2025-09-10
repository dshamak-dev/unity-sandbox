using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("Settings")]
    public float intensity;
    public bool isNightTime = false;
    public bool isDayTimeTime = false;
    public bool isEnabledOnStart = false;
    public Light[] lights;

    [Header("Debug")]
    public bool isOn = false;
    public LightManager lightM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         if (GameManager.Instance != null)
        {
            lightM = GameManager.Instance.GetComponent<LightManager>();
        }

        SwitchLights(isEnabledOnStart);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isNightTime && !isDayTimeTime)
        {
            return;
        }

        bool isNight = lightM != null ? !lightM.isDay : false;

        SwitchLights(isNight ? isNightTime : isDayTimeTime);
    }

    public void SwitchLights(bool input)
    {
        isOn = input;

        if (lights == null)
        {
            return;
        }

        foreach (Light l in lights)
        {
            l.enabled = input;
        }
    }
}
