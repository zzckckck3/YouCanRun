using System.Collections.Generic;
using UnityEngine;

public class TPSCharacterController : MonoBehaviour
{
    public Transform cameraArm;
    public Transform characterBody;
    // X축 최대 시야 제한
    private float maxCamX;

    void Update()
    {
        LookAround();
        FollowPlayer();
    }



    private void LookAround()
    {
        // 마우스의 X축과 Y축의 값
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // 카메라 앵글
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        maxCamX = camAngle.x - mouseDelta.y;

        if (maxCamX < 180f)
        {
            maxCamX = Mathf.Clamp(maxCamX, -1f, 50f);
        }
        else
        {
            maxCamX = Mathf.Clamp(maxCamX, 335f, 361f);
        }

        cameraArm.rotation = Quaternion.Euler(maxCamX, camAngle.y + mouseDelta.x, camAngle.z);
    }

    private void FollowPlayer()
    {
        transform.position = characterBody.position;
    }
}
