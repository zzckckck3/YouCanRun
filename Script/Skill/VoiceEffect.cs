using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceEffect : MonoBehaviour
{
    // Start is called before the first frame update
    private float VoiceTime;
    private int VoiceScaleCnt;
    void OnEnable()
    {
        VoiceTime=1.5f;
        VoiceScaleCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (VoiceTime > 0)
        {
            VoiceTime -= Time.deltaTime;
            if (VoiceScaleCnt % 2 == 1)
            {
                transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
            else
            {
                transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }
        }
        else { 
            gameObject.SetActive(false);
        }
    }



}
