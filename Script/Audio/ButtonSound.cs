using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip buttonClickSound; // 버튼 클릭 사운드

    private Button button;
    private AudioSource audioSource;

    void Start()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();

        // AudioSource 설정
        audioSource.clip = buttonClickSound;
        audioSource.playOnAwake = false;

        // 버튼 클릭 이벤트에 사운드 추가
        button.onClick.AddListener(PlayButtonClickSound);
    }

    public void PlayButtonClickSound()
    {
        audioSource.PlayOneShot(buttonClickSound);
    }
}
