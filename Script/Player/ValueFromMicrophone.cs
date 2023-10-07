using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ValueFromMicrophone : MonoBehaviour
{
    public AudioSource source;
    public PlayerAudio detector;
    public GameObject character;
    private PlayerController playerController;
    private PlayerStatus playerStatus;
    public float loudnessSensibility = 100;
    public float threshold = 0.1f;

    void Start()
    {
        playerController = character.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float loudness = detector.GetLoudnessFromMicrophone() * loudnessSensibility;
        //if (loudness < threshold)
        //    loudness = 0;
        //Debug.Log(loudness);
        if (loudness > 40) {
            playerStatus.ApplyItemEffect(3, 3, 40, 1, 3.5f);
        }
    }
}
