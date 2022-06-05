using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public float lives;
    public float maxLives;
    public float HP;
    public float maxHP;
    public float MP;
    public float maxMP;
    public float MP_Regen;
    public float LBCharge;
    public float playerMoveSpeed;
    public float counterCD;
    public float counterWindow;
    public float meleeDamage;
    public float counterDamage;
    public int meleeMaxHit;
    public int counterMaxHit;
    public int attackLV;
    public int counterLV;
    public int healthLV;
    public int moveSpeedLV;
    //==================================================//
    private float regenTick = 1f;
    private float LBTick = 2f;
    private float lastRegenTick;
    private float lastLBTick;

    // Start is called before the first frame update
    void Start()
    {
        StatsCheck();

        lives = maxLives;
        HP = maxHP;
        MP = maxMP;
        lastRegenTick = 0f;
        lastLBTick = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        //MP Regen
        if(Time.time > lastRegenTick + regenTick)
        {
            if(MP + MP_Regen < maxMP)
            {
                MP += MP_Regen;
            }
            else
            {
                MP = maxMP;
            }

            lastRegenTick = Time.time;
        }

        //LB Charge
        if (Time.time > lastLBTick + LBTick)
        {
            if (LBCharge + 1f < 100f)
            {
                LBCharge++;
            }
            else
            {
                LBCharge = 100f;
            }

            lastLBTick = Time.time;
        }

        StatsCheck();

    }

    public void StatsCheck()
    {
        switch (attackLV)
        {
            case 1:
                meleeDamage = 10;
                meleeMaxHit = 1;
                break;
            case 2:
                meleeDamage = 14;
                meleeMaxHit = 2;
                break;
            case 3:
                meleeDamage = 17;
                meleeMaxHit = 3;
                break;
            case 4:
                meleeDamage = 20;
                meleeMaxHit = 4;
                break;
            default:
                break;
        }

        switch (counterLV)
        {
            case 1:
                counterDamage = 30;
                counterWindow = 0.4f;
                counterMaxHit = 3;
                break;
            case 2:
                counterDamage = 40;
                counterWindow = 0.5f;
                counterMaxHit = 4;
                break;
            case 3:
                counterDamage = 50;
                counterWindow = 0.6f;
                counterMaxHit = 5;
                break;
            case 4:
                counterDamage = 60;
                counterWindow = 0.7f;
                counterMaxHit = 6;
                break;
            default:
                break;
        }

        switch (healthLV)
        {
            case 1:
                maxHP = 100;
                break;
            case 2:
                maxHP = 130;
                break;
            case 3:
                maxHP = 160;
                break;
            case 4:
                maxHP = 200;
                break;
            default:
                break;
        }

        switch (moveSpeedLV)
        {
            case 1:
                playerMoveSpeed = 4;
                break;
            case 2:
                playerMoveSpeed = 5;
                break;
            case 3:
                playerMoveSpeed = 6;
                break;
            case 4:
                playerMoveSpeed = 7;
                break;
            default:
                break;
        }
    }
}
