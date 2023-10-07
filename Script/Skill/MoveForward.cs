using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float moveSpeed = 1.0f;

    private void Update()
    {
        Vector3 forward = transform.forward;
        Vector3 moveAmount = forward * moveSpeed * Time.deltaTime;

        transform.position += moveAmount;
    }
}
