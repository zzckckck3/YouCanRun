using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetSetting : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider Masterslider;
    public Slider musicSlider;
    public Slider effectSlider;
    public Slider mouseSlider;

    private float masterValue;
    private float musicValue;
    private float effectValue;
    private float mouseValue;

    public delegate void MouseSensitivityChanged(float newSensitivity);
    public static event MouseSensitivityChanged OnMouseSensitivityChanged;

    void Start()
    {
        Masterslider.value = PlayerPrefs.GetFloat("MasterSound", 0.75f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicSound", 0.75f);
        effectSlider.value = PlayerPrefs.GetFloat("EffectSound", 0.75f);
        mouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 0.75f);

        masterValue = PlayerPrefs.GetFloat("MasterSound", 0.75f);
        musicValue = PlayerPrefs.GetFloat("MusicSound", 0.75f);
        effectValue = PlayerPrefs.GetFloat("EffectSound", 0.75f);
        mouseValue = PlayerPrefs.GetFloat("MouseSensitivity", 0.75f);
    }

    public void ControllMaster(float sliderValue)
    {
        mixer.SetFloat("Master", Mathf.Log10(sliderValue) * 20);
        masterValue = sliderValue;
    }
    public void ControllMusic(float sliderValue)
    {
        mixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
        musicValue = sliderValue;
    }
    public void ControllEffect(float sliderValue)
    {
        mixer.SetFloat("Effect", Mathf.Log10(sliderValue) * 20);
        effectValue = sliderValue;
    }
    public void ControllMouse(float sliderValue)
    {
        mouseValue = sliderValue;
    }
    public void CheckSetting()
    {
        PlayerPrefs.SetFloat("MasterSound", masterValue);
        PlayerPrefs.SetFloat("MusicSound", musicValue);
        PlayerPrefs.SetFloat("EffectSound", effectValue);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseValue);

        OnMouseSensitivityChanged?.Invoke(mouseValue);

        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 락
    }
    public void CheckSettingAtMainUi()
    {
        PlayerPrefs.SetFloat("MasterSound", masterValue);
        PlayerPrefs.SetFloat("MusicSound", musicValue);
        PlayerPrefs.SetFloat("EffectSound", effectValue);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseValue);

        OnMouseSensitivityChanged?.Invoke(mouseValue);

        gameObject.SetActive(false);
    }

    public void UnCheckSetting()
    {
        mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("MasterSound", 0.75f)) * 20);
        mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("MusicSound", 0.75f)) * 20);
        mixer.SetFloat("Effect", Mathf.Log10(PlayerPrefs.GetFloat("EffectSound", 0.75f)) * 20);
        Masterslider.value = PlayerPrefs.GetFloat("MasterSound", 0.75f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicSound", 0.75f);
        effectSlider.value = PlayerPrefs.GetFloat("EffectSound", 0.75f);
        mouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 0.75f);

        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 락
    }

    public void UnCheckSettingInMainUi()
    {
        mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("MasterSound", 0.75f)) * 20);
        mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("MusicSound", 0.75f)) * 20);
        mixer.SetFloat("Effect", Mathf.Log10(PlayerPrefs.GetFloat("EffectSound", 0.75f)) * 20);
        Masterslider.value = PlayerPrefs.GetFloat("MasterSound", 0.75f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicSound", 0.75f);
        effectSlider.value = PlayerPrefs.GetFloat("EffectSound", 0.75f);
        mouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 0.75f);

        gameObject.SetActive(false);
    }
}
