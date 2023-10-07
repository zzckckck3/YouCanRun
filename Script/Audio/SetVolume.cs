using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("BackGroundMusic", 0.75f);
    }
    public void SerLevel(float sliderValue)
    {
        mixer.SetFloat("BackGroundMusic", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("BackGroundMusic", sliderValue);
    }
}
