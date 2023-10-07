using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUiKeyManager : MonoBehaviour
{
    public GameObject escapeWindow;
    public SetSetting setSetting;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEscapeWindow();
        }
    }

    void ToggleEscapeWindow()
    {
        escapeWindow.SetActive(!escapeWindow.activeSelf);

        if (escapeWindow.activeSelf == false)
        {
            setSetting.UnCheckSettingInMainUi();
        }

    }
}
