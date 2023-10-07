using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostTime : MonoBehaviour
{

    private float frostTime;
    public GameObject FrostObject;
    private void OnEnable()
    {
        frostTime = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        TimeFlow();
    }
    void TimeFlow() { 
        frostTime-= Time.deltaTime;
        if (frostTime < 0) {
            FrostObject.SetActive(false);
        }
    }
}   
