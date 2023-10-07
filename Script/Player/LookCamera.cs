using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{

    //Vector3 startScale;
    //public float distance = 3;

    void Update()
    {
        //float dist = Vector3.Distance(camera.transform.position, transform.position);
        //Vector3 newScale = startScale * dist / distance;
        //transform.localScale = newScale;
        transform.rotation = Camera.main.transform.rotation;
    }
}
