using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool facingRight;
    [SerializeField] private bool isResInvuln = false;
    [SerializeField] private bool isInvuln = false;
    [SerializeField] private bool isAlive = true;
    //===============================================//
    private Rigidbody2D myRigidbody;
    private Animator myAnimator;
    private CapsuleCollider2D myCollider2D;
    private Vector3 inputVector;
    private float invulnTime;
    //===============================================//
    [Header("Input Checks")]
    //===============================================//
    [SerializeField] private bool allowInput = true;
    [SerializeField] private bool dashExecuted = false;
    //===============================================//
    [Header("Counter")]
    //===============================================//
    [SerializeField] private bool counterStance = false;
    [SerializeField] private bool canCounter = true;
    [SerializeField] private float counterStartTime;
    //===============================================//
    [Header("Dashing")]
    //===============================================//
    [SerializeField] private bool dashing = false;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashExecutionTime;
    [SerializeField] private float dashDuration;
    [SerializeField] private Vector3 dashDir;
    //===============================================//
    [Header("Attack")]
    //===============================================//
    [SerializeField] private bool normalAttacking;
    [SerializeField] private bool specialAttacking;
    [SerializeField] private float NAStartTime;
    [SerializeField] private float SAStartTime;
    //===============================================//
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private LayerMask attackLayerMask;
    //===============================================//
    public GameObject deathKanji;
    [SerializeField] private GameObject targetArrow;
    [SerializeField] private GameObject meleePrefab;
    [SerializeField] private GameObject counterPrefab;
    [SerializeField] private GameObject counterSphere;
    [SerializeField] private GameObject resInvulnSphere;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCollider2D = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveSpeed = GameManager.Instance.playerData.playerMoveSpeed;

        targetArrow.transform.eulerAngles = new Vector3(0, 0, MirageUtilities.HelperFunctions.GetAngleFromVector(MirageUtilities.HelperFunctions.GetMouseWorldPosition() - targetArrow.transform.position) - 90);

        if (targetArrow.transform.eulerAngles.z > 0 && targetArrow.transform.eulerAngles.z < 180)
        {
            facingRight = false;
        }
        else
        {
            facingRight = true;
        }

        GetComponent<SpriteRenderer>().flipX = !facingRight;

        CounterCheck();

        if(isResInvuln)
        {
            if(Time.time > invulnTime + 2f)
            {
                resInvulnSphere.SetActive(false);
                isInvuln = false;
                isResInvuln = false;
            }
        }
    }

    void FixedUpdate()
    {
        if(!dashing && !counterStance)
        {
            Vector2 playerVelocity = inputVector * moveSpeed;

            myRigidbody.velocity = playerVelocity;

            if(inputVector != Vector3.zero)
            {
                myAnimator.SetBool("Running", true);
            }
        }

        AttackCheck();
        DashMovement();
    }

    public void Death()
    {
        myAnimator.SetBool("Death", true);
        deathKanji.SetActive(true);
        inputVector = Vector2.zero;
        allowInput = false;
        isAlive = false;
    }

    public void Resurrection()
    {
        myAnimator.SetBool("Death", false);
        deathKanji.SetActive(false);
        allowInput = true;
        isAlive = true;
        Debug.Log("test");
        isInvuln = true;
        isResInvuln = true;
        resInvulnSphere.SetActive(true);
        invulnTime = Time.time;
    }

    public void Movement(InputAction.CallbackContext context)
    {
        if (!dashing && isAlive)
        {
            if (context.performed)
            {
                inputVector = context.ReadValue<Vector2>();
            }
            if (context.canceled)
            {
                inputVector = Vector3.zero;
                myAnimator.SetBool("Running", false);
            }
        }
    }

    public void Counter(InputAction.CallbackContext context)
    {
        if(allowInput)
        {
            if (context.performed && canCounter)
            {
                myAnimator.SetBool("Countering", true);
                counterStance = true;
                canCounter = false;
                counterStartTime = Time.time;
                counterSphere.SetActive(true);
                allowInput = false;
                GameManager.Instance.counterCDTime = GameManager.Instance.playerData.counterCD;
                GameManager.Instance.counterCD_UI.SetActive(true);
            }
        }           
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (allowInput)
        {
            //add float cooldown at end if necessary
            if (context.performed && !dashing && !dashExecuted && Time.time > dashExecutionTime)
            {
                dashExecuted = true;
                //GameObject dashEffect = Instantiate(Resources.Load<GameObject>("PlayerDash/DashEffect"), gameObject.transform);
                
                //dashEffect.transform.eulerAngles = new Vector3(0, 0, MirageUtilities.HelperFunctions.GetAngleFromVector(inputVector.normalized));
                //Destroy(dashEffect, 0.5f);
            }
            if (context.canceled)
            {
                dashExecuted = false;
            }
        }
    }

    public void NormalAttack(InputAction.CallbackContext context)
    {
        if (allowInput)
        {
            if (context.performed)
            {
                if (!normalAttacking && !specialAttacking && Time.time > NAStartTime + 0.5f) //last variable is attack interval
                {
                    normalAttacking = true;

                    myAnimator.SetTrigger("Attack");

                    NAStartTime = Time.time;

                    GameObject go = Instantiate(meleePrefab);
                    Vector3 angle = new Vector3(0, 0, MirageUtilities.HelperFunctions.GetAngleFromVector(MirageUtilities.HelperFunctions.GetMouseWorldPosition() - targetArrow.transform.position));
                    go.GetComponent<MeleeBase>().Initialize(MirageUtilities.HelperFunctions.GetMouseWorldPosition() - targetArrow.transform.position, angle, transform.position, GameManager.Instance.playerData.meleeDamage, GameManager.Instance.playerData.meleeMaxHit, this.gameObject);
                }
            }
        }
    }

    private void DashMovement()
    {
        if (dashExecuted)
        {
            dashDir = new Vector3(inputVector.x, inputVector.y).normalized;

            dashing = true;
            dashExecuted = false;
            allowInput = false;
            dashExecutionTime = Time.time;
        }

        if (dashing)
        {
            myRigidbody.velocity = dashDir * dashSpeed;
            CreateAfterimage();
            isInvuln = true;
            if (Time.time >= dashExecutionTime + dashDuration)
            {
                dashing = false;
                isInvuln = false;
                myRigidbody.velocity = Vector3.zero;

                allowInput = true;
            }
        }
    }

    private void AttackCheck()
    {
        if (normalAttacking && Time.time > NAStartTime + 0.5f)
        {
            normalAttacking = false;
        }

        //if (specialAttacking && Time.time > SAStartTime + 0.7f)
        //{
        //    GameObject swordWave = Instantiate(swordWavePrefab, transform.position, Quaternion.identity);

        //    //if (facingRight)
        //    //{
        //    //    swordWave.GetComponent<ProjectileBase>().Initialize(Vector3.right, new Vector3(0, 0, 0), transform.position + new Vector3(1, 0.5f, 1), PL_Player.Instance.specialAttackDamage);
        //    //}
        //    //else
        //    //{
        //    //    swordWave.GetComponent<ProjectileBase>().Initialize(Vector3.left, new Vector3(0, 180, 0), transform.position + new Vector3(-1, 0.5f, 1), PL_Player.Instance.specialAttackDamage);
        //    //}

        //    specialAttacking = false;
        //}
    }

    private void CounterCheck()
    {
        if(counterStance)
        {
            myRigidbody.velocity = Vector2.zero;
            if (Time.time > counterStartTime + GameManager.Instance.playerData.counterWindow)
            {
                counterStance = false;
                allowInput = true;
                myAnimator.SetBool("Countering", false);
                counterSphere.SetActive(false);
            }
        }

        if(!canCounter)
        {
            if(Time.time > counterStartTime + GameManager.Instance.playerData.counterCD)
            {
                canCounter = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dashing)
        {
            myRigidbody.velocity = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        
    }

    private void DrawAttackCubeGizmo()
    {
        Gizmos.color = Color.red;
        if (facingRight)
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(1f, 0.25f), new Vector3(2, 1.5f, 0));
        }
        else
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(-1f, 0.25f), new Vector3(2, 1.5f, 0));
        }
    }

    private void CreateAfterimage()
    {
        GameObject afterimage = Instantiate(Resources.Load<GameObject>("PlayerDash/PlayerAfterimage"), transform.position, Quaternion.identity);
        afterimage.GetComponent<SpriteAfterimage>().Initialize(GetComponent<SpriteRenderer>().sprite, GetComponent<SpriteRenderer>().flipX);
    }


    public void InflictDamage(float damageToInflict)
    {
        if(counterStance)
        {
            GameObject go = Instantiate(counterPrefab);
            Vector3 angle = new Vector3(0, 0, MirageUtilities.HelperFunctions.GetAngleFromVector(MirageUtilities.HelperFunctions.GetMouseWorldPosition() - targetArrow.transform.position));
            go.GetComponent<MeleeBase>().Initialize(MirageUtilities.HelperFunctions.GetMouseWorldPosition() - targetArrow.transform.position, angle, transform.position, GameManager.Instance.playerData.counterDamage, GameManager.Instance.playerData.counterMaxHit, this.gameObject);
            myAnimator.SetTrigger("Counter");
            counterSphere.SetActive(false);
        }
        else if(isInvuln)
        {
            return;
        }
        else
        {
            GameManager.Instance.playerData.HP -= damageToInflict;
            GameObject bloodEffect = Instantiate(Resources.Load<GameObject>("HitEffects/Blood_HitEffect/Blood_HitEffect"), gameObject.transform);
            GameObject damageNumber = Instantiate(Resources.Load<GameObject>("DamageNumber"), transform.position, Quaternion.identity);
            damageNumber.GetComponent<FloatingText>().textString = "- " + damageToInflict.ToString();
        }
    }
}
