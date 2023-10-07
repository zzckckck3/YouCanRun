using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayerCharacter : MonoBehaviour
{
    public SelectClass select;
    private void OnDisable()
    {
        select.GetCharacterClass();
    }
}
