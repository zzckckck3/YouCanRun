using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int indexNumber;

    public enum Type
    {
        Boost,
        Wall,
        Slow,
        HyperKineticPositionReverser,  // 초동역학위치전환기
        Bomb,
        ResetClass,
        ObstacleReset
    };

    public Type type;

    private void Awake()
    {
        type = (Type)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Type)).Length);  // 아이템의 타입을 랜덤으로 생성
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
}