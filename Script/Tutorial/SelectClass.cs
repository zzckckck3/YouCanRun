using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectClass : MonoBehaviour
{
    public CharacterClass characterClass;
    public GameObject classPrevButton;
    public GameObject classNextButton;
    public Image classImage;
    public TextMeshProUGUI className;
    private int classNum = 0;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            PrevClass();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            NextClass();
        }
    }

    public void GetCharacterClass()
    {
        characterClass = GameObject.Find("PlayerCharacter(Clone)").GetComponent<CharacterClass>();
    }

    public void PrevClass()
    {
        if (classNum > 0)
        {
            classNum--;
            characterClass.SelectClass(classNum);
            classImage.sprite = Resources.Load("ClassImage/" + characterClass.ClassTypes[classNum].ToString(), typeof(Sprite)) as Sprite;
            className.text = characterClass.ClassTypes[classNum].ToString();
            if(classNextButton.active.Equals(false))
            {
                classNextButton.SetActive(true);
            }
        }
        if(classNum == 0)
        {
            classPrevButton.SetActive(false);
        }

    }

    public void NextClass()
    {
        if (classNum < characterClass.ClassTypes.Length-1)
        {
            classNum++;
            characterClass.SelectClass(classNum);
            classImage.sprite = Resources.Load("ClassImage/" + characterClass.ClassTypes[classNum].ToString(), typeof(Sprite)) as Sprite;
            className.text = characterClass.ClassTypes[classNum].ToString();
            if (classPrevButton.active.Equals(false))
            {
                classPrevButton.SetActive(true);
            }
        }
        if(classNum == characterClass.ClassTypes.Length-1)
        {
            classNextButton.SetActive(false);
        }
    }
}
