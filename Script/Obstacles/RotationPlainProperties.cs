using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPlainProperties : MonoBehaviour
{
    public Vector3 _rotation = new Vector3(0,120,0);

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);
    }
}
