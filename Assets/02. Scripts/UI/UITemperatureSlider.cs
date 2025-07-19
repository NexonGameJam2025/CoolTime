using Core.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITemperatureSlider : UIBase
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI textValue;
    
    private static float START_TEMPERATURE = 35.0f;
    private static float MAX_TEMPERATURE = 100.0f;

    private void Start()
    {
        slider.value = START_TEMPERATURE / 100.0f;
        textValue.text = $"{START_TEMPERATURE:F1} °C";
    }
    
    public void SetTemperature(float temperature)
    {
        if (temperature < 0.0f || temperature >= MAX_TEMPERATURE)
        {
            Debug.Log($"UITemperatureSlider.SetValue: value {temperature} is out of range (0.0 - {MAX_TEMPERATURE})");
            return;
        }
        
        textValue.text = $"{temperature:F1} °C";
        var parsedValue = temperature / 100.0f;
        slider.value = parsedValue;
    }
}
