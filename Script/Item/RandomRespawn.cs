using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomRespawn : MonoBehaviour
{
    public GameObject[] rangeObjects;  // 아이템매니저에서 관리할 아이템 생성 구역들
    public GameObject randomItem;  // 아이템박스

    Vector3 originPosition;
    List<Vector3> respawnPositions = new List<Vector3>();
    public bool[] respawnCheck;


    private void Awake()
    {
        // 생성 구역들의 위치를 리스트안에 저장
        foreach (GameObject rangeObject in rangeObjects)
        {
            originPosition = rangeObject.transform.position;
            Vector3 respawnPosition = new Vector3(originPosition.x, originPosition.y + 1f, originPosition.z);
            respawnPositions.Add(respawnPosition);
        }

        // 생성 여부를 체크하기  위해서 생성구역의 개수만큼의 false리스트를 생성
        respawnCheck = new bool[rangeObjects.Length];
        for (int i = 0; i < respawnCheck.Length; i++)
        {
            respawnCheck[i] = false;
        }
    }

    private void Start()
    {
        StartCoroutine(RandomRespawn_Coroutine());
    }

    public IEnumerator RandomRespawn_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            if (!respawnCheck.Contains(false))
            {
                break;
            }

            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, respawnCheck.Length);
            }
            while (respawnCheck[randomIndex] == true);

            Vector3 respawnPoint = respawnPositions[randomIndex];
            GameObject instantItem = Instantiate(randomItem, respawnPoint, Quaternion.identity);
            instantItem.GetComponent<Item>().indexNumber = randomIndex;

            respawnCheck[randomIndex] = true;
        }
    }
}
