using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_Player : MonoBehaviour
{
    [Header("Stats")]
    //==================================================//
    public float playerHP;
    public float playerMaxHP;
    public float playerSP;
    public float playerMaxSP;
    public int potionCharges;
    public int maxPotionCharges;
    public int goldAmount;
    [Header("Jump")]
    //==================================================//
    public int jumpCharges;
    [SerializeField] private float jumpRecharge;
    [SerializeField] private float jumpCooldown;
    //==================================================//
    [Header("Dash")]
    //==================================================//
    [SerializeField] private float dashCharges;
    [SerializeField] private float dashRecharge;
    public float dashCooldown;
    //==================================================//
    [Header("Normal Attack")]
    //==================================================//
    public float attackInterval;
    public float normalAttackDamage;
    [Header("Special Attack")]
    //==================================================//
    public bool specialUsed;
    public float specialAttackDamage;
    public float specialCooldown;
    private float elapsedTime;
    //==================================================//
    //==================================================//
    private static PL_Player _instance;

    public static PL_Player Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHP = playerMaxHP;
        playerSP = playerMaxSP;
        elapsedTime = 0f;
        potionCharges = 0;
        goldAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(specialUsed)
        {
            elapsedTime += 1f * Time.deltaTime;

            if(elapsedTime >= specialCooldown)
            {
                elapsedTime = 0f;
                specialUsed = false;
            }
        }
    }
}
