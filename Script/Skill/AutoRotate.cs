using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotation;

    public Space space = Space.Self;

    void Update()
    {
        this.transform.Rotate(rotation * Time.deltaTime, space);
    }
}
