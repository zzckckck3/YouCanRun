using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.UI;

public class ScarecrowStatus : MonoBehaviour
{
    private int hp = 20;
    public DamageNumber damageNum;
    public DamageNumber healNum;
    public Image hpBar;

    void Update()
    {
        if(hp <= 0)
        {
            hp = 20;
        }
    }
}
