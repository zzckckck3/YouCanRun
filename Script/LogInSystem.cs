using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LogInSystem : MonoBehaviour
{
    public GameObject loginCanvas;
    public GameObject signUpCanvas;
    public GameObject loadingCanvas;
    public TMP_InputField signUpId;
    public TMP_InputField signUpPassword;
    public TMP_InputField nickname;
    public TMP_InputField loginId;
    public TMP_InputField loginPassword;

    public static int loginResult;
   
    // Start is called before the first frame update
    void Start()
    {
        FirebaseAuthManager.Instance.Init();
    }

    public void Create()
    {
        FirebaseAuthManager.Instance.Create(signUpId.text, signUpPassword.text, nickname.text);
    }
    public void GoLogin()
    {
        loginCanvas.SetActive(true);
        signUpCanvas.SetActive(false);
    }

    public void LoginSucess()
    {
        SceneManager.LoadScene("UI");
    }
}
