using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorTexture : MonoBehaviour
{
    public float ScrollX = 0.0f;
    public float ScrollY = -0.2f;

    private Material material; // 캐싱할 Material 변수

    private void Start()
    {
        material = GetComponent<Renderer>().material; // Start에서 Material 할당
    }

    // Update is called once per frame
    void Update()
    {
        float OffsetX = Time.time * ScrollX;
        float OffsetY = Time.time * ScrollY;
        material.mainTextureOffset = new Vector2(OffsetX, OffsetY); // 캐싱된 Material 사용
    }
}
