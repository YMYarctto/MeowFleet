using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider_Volumn : MonoBehaviour
{
    public SliderType sliderType;
    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void OnEnable() 
    {
        if(ResourceManager.IsInit)
        {
            LoadSliderValue();
        }
    }

    public void OnSliderChange()
    {
        if(sliderType == SliderType.音乐)
        {
            AudioManager.instance.MusicVolume = slider.value;
        }
        else if(sliderType == SliderType.音效)
        {
            AudioManager.instance.AudioVolume = slider.value;
        }
    }

    public void LoadSliderValue()
    {
        if (sliderType == SliderType.音乐)
        {
            slider.value = AudioManager.instance.MusicVolume;
        }
        else if (sliderType == SliderType.音效)
        {
            slider.value = AudioManager.instance.AudioVolume;
        }
        Debug.Log($"LoadSliderValue: {slider.value}");
    }

    public enum SliderType
    {
        音乐,
        音效,
    }
}