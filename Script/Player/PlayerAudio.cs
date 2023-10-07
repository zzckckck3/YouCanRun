using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    // 음성입력을 감지할 스크립트
    public AudioClip microphoneClip;
    private int sampleWindow=64; 
    void Start()
    {
        MicrophoneToAudioClip();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void MicrophoneToAudioClip()
    {
        // 마이크 가져오기
        string microphoneName = Microphone.devices[0];

        // 마이크에서 오디오 Clip 가져오기
        //microphoneClip = Microphone.Start(microphoneName,true,1,AudioSettings.outputSampleRate);
        microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);

    }
    public float GetLoudnessFromMicrophone() {
        //Debug.Log("포지션이 뭔데? "+Microphone.GetPosition(Microphone.devices[0]));

        return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]),microphoneClip);
    }
        public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - sampleWindow;

        if (startPosition < 0)
        {
            return 0;
        }
        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);
        //Debug.Log("클립 : "+ waveData);

        float totalLoudness = 0;
        for(int i= 0; i<sampleWindow; i++) {

            totalLoudness += Mathf.Abs(waveData[i]);
        }

        return totalLoudness / sampleWindow;
    }
}


//aud.GetData(samples, 0);
//float sum = 0;
//for (int i = 0; i < samples.Length; i++)
//{
//    sum += samples[i] * samples[i];
//}
//rmsValue = Mathf.Sqrt(sum / samples.Length);
//rmsValue = rmsValue * modulate;
//rmsValue = Mathf.Clamp(rmsValue, 0, 100);
//resultValue = Mathf.RoundToInt(rmsValue);
//if (resultValue < cutValue)
//{
//    resultValue = 0;
//}