using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneToTutorial : MonoBehaviour
{
    public void GotoTutorial()
    {

        //SceneManager.LoadScene("Lobby");
        SceneManager.LoadScene("Tutorial");
    }
}
