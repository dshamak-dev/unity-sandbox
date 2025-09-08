using UnityEngine;

public class LightManager : MonoBehaviour
{
    public float dayDuration = 100f;
    public float dayLightPercentage = 60f;
    public float lightTransition = 0.001f;
    public float minLight = 0.05f;
    public float minTemperature = 2000f;
    public float maxLight = 0.5f;
    public float maxTemperature = 5000f;

    public Light sunSource;

    public int day = 0;
    public float duration = 0;
    public float daylightDuration = 0f;
    public float lightIntensity = 0.5f;
    public float lightTemperature = 0;
    public bool isDay = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightIntensity = maxLight;
        lightTemperature = maxTemperature;

        daylightDuration = dayDuration * (dayLightPercentage / 100);
    }

    // Update is called once per frame
    void Update()
    {
        Calculate();

        if (sunSource)
        {
            sunSource.intensity = lightIntensity;
            sunSource.colorTemperature = lightTemperature;
        }
    }

    void Calculate()
    {
        duration += Time.deltaTime;

        if (duration >= dayDuration)
        {
            duration = 0;
            day++;
        }

        isDay = duration <= daylightDuration;

        lightIntensity = Mathf.Lerp(lightIntensity, isDay ? maxLight : minLight, lightTransition);
        lightTemperature = Mathf.Lerp(lightTemperature, isDay ? maxTemperature : minTemperature, lightTransition);
    }
}
