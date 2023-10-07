using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallUp : MonoBehaviour
{
    float riseSpeed = 15.0f;
    float targetPosition;

    void Start()
    {
        targetPosition = transform.position.y + 2;
    }

    void Update()
    {
        // 벽의 y축위치가 목표 y축위치보다 작으면 올라오도록 설정
        if (transform.position.y < targetPosition)
        {
            transform.position += new Vector3(0, riseSpeed * Time.deltaTime, 0);
        }
        Destroy(gameObject, 20);
    }
}
