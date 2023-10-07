using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGetItem : MonoBehaviour
{
    public Item myItem;
    GameObject nearObject;
    PlayerController playerController; // 아이템으로 플레이어의 상태를 조절하기 위함
    public float itemCooldown;
    public int bullet;
    public Sprite itemImage;
    public bool hasItem;
    public Animator anim;
    private GameObject effect;
    PhotonView PV;
    RandomRespawn randomRespawn;
    GameObject itemManager;
    public AudioSource box;
    public AudioClip openBox;

    void Start()
    {
        playerController = transform.GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>();
        itemManager = GameObject.Find("ItemManager");
        randomRespawn = itemManager.GetComponent<RandomRespawn>();
        hasItem = false;
    }

    IEnumerator HideAfterSeconds(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        PV.RPC("GetBoxRPC", RpcTarget.All);
        obj.SetActive(false);
    }

    public void GetItem()
    {

        if (myItem == null && nearObject != null && !hasItem)  // e버튼을 누르고, 아이템 창이 비어있고, 가까이에 아이템이 있으면
        {
            myItem = nearObject.GetComponent<Item>();
            anim = nearObject.GetComponentInChildren<Animator>();
            hasItem = true;
            switch (myItem.type)
            {
                case Item.Type.Boost:
                    {
                        bullet = 1;
                        itemImage = Resources.Load<Sprite>("Item/Image/Boost");
                        Debug.Log("부스트");
                        break;
                    }
                case Item.Type.Wall:
                    {
                        itemCooldown = 5;
                        bullet = 2;
                        itemImage = Resources.Load<Sprite>("Item/Image/Wall");
                        Debug.Log("벽");
                        break;
                    }
                case Item.Type.Slow:
                    {
                        bullet = 1;
                        itemImage = Resources.Load<Sprite>("Item/Image/Slow");
                        Debug.Log("느림");
                        break;
                    }
                case Item.Type.HyperKineticPositionReverser:
                    {
                        Debug.Log("위치변환");
                        bullet = 1;
                        itemImage = Resources.Load<Sprite>("Item/Image/PositionReverse");
                        break;
                    }
                case Item.Type.Bomb:
                    {
                        Debug.Log("폭탄");
                        itemCooldown = 3;
                        bullet = 5;
                        itemImage = Resources.Load<Sprite>("Item/Image/Bomb");
                        break;
                    }
                case Item.Type.ResetClass:
                    {
                        Debug.Log("직업 초기화");
                        bullet = 1;
                        itemImage = Resources.Load<Sprite>("Item/Image/ResetClass");
                        break;
                    }
                case Item.Type.ObstacleReset:
                    {
                        bullet = 1;
                        itemImage = Resources.Load<Sprite>("Item/Image/ResetObstacle");
                        break;
                    }
            }
            anim.SetBool("isGet", true);
            box.clip = openBox;
            box.Play();
            playerController.itemCooldown = 0;
            playerController.bullet = bullet;
            int itemNumber = nearObject.GetComponent<Item>().indexNumber;
            StartCoroutine(HideAfterSeconds(nearObject, 0.2f));
            Debug.Log(randomRespawn);
            randomRespawn.respawnCheck[itemNumber] = false;
            StartCoroutine(randomRespawn.RandomRespawn_Coroutine());
            nearObject = null;
        }
    }

    void OnTriggerStay(Collider other)  // 가까이에 들어온 Item이 trigger에 반응하면 nearObject를 변경
    {
        if (other.tag == "Item")
        {
            nearObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Item")
        {
            StartCoroutine(ResetNearObjectAfterDelay(0.2f));
        }
    }

    IEnumerator ResetNearObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        nearObject = null;
    }

    [PunRPC]
    public void GetBoxRPC()
    {
        // 폭발 파티클
        effect = Resources.Load("Effects/CFXR Firework 2") as GameObject;
        GameObject instance = Instantiate(effect, nearObject.transform.position, Quaternion.identity);
        Destroy(instance, 2.5f); // 2.5초 후에 인스턴스 파괴
    }
}

