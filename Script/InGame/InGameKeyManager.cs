using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameKeyManager : MonoBehaviour
{
    public GameObject escapeWindow;
    public SetSetting setSetting;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 락
        
    }
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
        
        if(escapeWindow.activeSelf == true)
        {   
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // 마우스 커서 락
        } else if (escapeWindow.activeSelf == false)
        {
            setSetting.UnCheckSetting();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 락
        }
        
    }
}
