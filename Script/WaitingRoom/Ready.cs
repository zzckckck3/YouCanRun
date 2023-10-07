using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ready : MonoBehaviour
{
    public GameObject readyBtn;
    public GameObject readyLight;

    private Renderer rend;
    private Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        // 게임 오브젝트의 Renderer 컴포넌트 가져오기
        rend = GetComponent<Renderer>();
        materials = rend.materials;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if(readyLight.activeSelf)
            {
                readyLight.SetActive(false);
                rend.material = materials[1];
            }
            else
            {
                readyLight.SetActive(true);
                rend.material = materials[0];
            }
            RoomPhotonManager.Instance.Ready();
        }
    }
}
