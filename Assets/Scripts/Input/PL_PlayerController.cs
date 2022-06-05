using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PL_PlayerController : MonoBehaviour, IDamageable
{
    //===============================================//
    [SerializeField] private bool facingRight;
    [SerializeField] private bool grounded = false;
    [SerializeField] private bool ungrounded = false;
    [SerializeField] private bool coyotePossible = false;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float staggerTime;
    [SerializeField] private float timeLeftGrounded;
    //===============================================//
    [Header("Horizontal Movement")]
    //===============================================//
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveAcceleration;
    //===============================================//   
    [Header("Vertical Movement")]
    //===============================================//
    [SerializeField] private bool jumping = false;
    [SerializeField] private int jumpCount;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpFalloff;
    [SerializeField] private float coyoteTime = 0.2f;
    [Header("Dashing")]
    //===============================================//
    [SerializeField] private bool dashing;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashExecutionTime;
    [SerializeField] private float dashDuration;
    [SerializeField] private Vector3 dashDir;
    //===============================================//
    [Header("Attack")]
    //===============================================//
    [SerializeField] private bool normalAttacking;
    [SerializeField] private bool specialAttacking;
    [SerializeField] private int NACounter;
    [SerializeField] private float NAStartTime;
    [SerializeField] private float SAStartTime;
    [Header("Block")]
    //===============================================//
    [SerializeField] private bool blocking;
    //===============================================//
    [Header("Input Checks")]
    //===============================================//
    [SerializeField] private bool allowInput = true;
    [SerializeField] private bool dashExecuted = false;
    [SerializeField] private bool jumpExecuted = false;
    [SerializeField] private bool moveRight = false;
    [SerializeField] private bool moveLeft = false;
    //===============================================//
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private LayerMask attackLayerMask;
    [SerializeField] private Collider2D[] hitEntities = new Collider2D[0];
    [Header("Prefabs")]
    //===============================================//
    [SerializeField] private GameObject swordWavePrefab;
    [SerializeField] private ParticleSystem jumpDust;
    //===============================================//
    private Rigidbody2D myRigidbody2D;
    private Animator myAnimator;
    //===============================================//
    private Vector2 inputAxis = new Vector2(0, 0);
    private Vector2 moveDistance = new Vector2(0, 0);
    private Vector3 facingDir = new Vector3(0, 0, 0);
    //===============================================//

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(!normalAttacking)
        {
            GetComponent<SpriteRenderer>().flipX = !facingRight;

            if(facingRight)
            {
                facingDir = new Vector3(1, 0, 1);
            }
            else
            {
                facingDir = new Vector3(-1, 0, -1);
            }
        }

        grounded = Physics2D.OverlapCircle(transform.position + new Vector3(0, -0.4f), 0.2f, collisionLayerMask);

        if (!grounded && !ungrounded)
        {
            ungrounded = true;
            coyotePossible = true;
            timeLeftGrounded = Time.time;
        }

        if (grounded)
        {
            jumpCount = 0;
            ungrounded = false;
        }

        if (coyotePossible)
        {
            if (Time.time > timeLeftGrounded + coyoteTime)
            {
                ungrounded = false;
                coyotePossible = false;
            }
        }

        if (grounded && jumping)
        {
            jumping = false;
            myAnimator.SetBool("IsFalling", false);
            myAnimator.SetBool("IsJumping", jumping);
        }

        if (grounded && !jumping)
        {
            myAnimator.SetBool("IsFalling", false);
        }

        if (!grounded && jumping)
        {
            myAnimator.SetBool("IsFalling", false);
        }

        if (!grounded && !jumping)
        {
            myAnimator.SetBool("IsFalling", true);
        }


        AttackCheck();
        HorizontalMovement();
        DashMovement();
        VerticalMovement();
    }

    private void OnDrawGizmos()
    {
        DrawGroundCircleGizmo();
        DrawAttackCubeGizmo();
    }

    private void DrawGroundCircleGizmo()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, -0.4f), 0.2f);
    }

    private void DrawAttackCubeGizmo()
    {
        Gizmos.color = Color.red;
        if(facingRight)
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(1f, 0.25f), new Vector3(2, 1.5f, 0));
        }
        else
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(-1f, 0.25f), new Vector3(2, 1.5f, 0));
        }
    }


    public void Move(InputAction.CallbackContext context)
    {
        if(allowInput)
        {
            Vector2 xDir = context.ReadValue<Vector2>();
            if (context.performed && xDir.x > 0)
            {
                inputAxis = context.ReadValue<Vector2>();
                facingRight = true;
                moveRight = true;
                moveLeft = false;
                NACounter = 0;
            }
            if (context.performed && xDir.x < 0)
            {
                inputAxis = context.ReadValue<Vector2>();
                facingRight = false;
                moveRight = false;
                moveLeft = true;
                NACounter = 0;
            }
            if (context.canceled)
            {
                inputAxis = Vector2.zero;
                moveRight = false;
                moveLeft = false;
            }
        }       
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (allowInput)
        {
            if (context.performed && !jumpExecuted && grounded)
            {
                jumpExecuted = true;
                CreateJumpEffect();
            }
            else if (context.performed && !jumpExecuted && coyotePossible)
            {
                jumpExecuted = true;
                CreateJumpEffect();
            }

            if (context.canceled)
            {
                jumpExecuted = false;
            }
        }       
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (allowInput)
        {
            if (context.performed && !dashing && !dashExecuted && Time.time > dashExecutionTime + PL_Player.Instance.dashCooldown)
            {
                dashExecuted = true;
                GameObject dashEffect = Instantiate(Resources.Load<GameObject>("PlayerDash/DashEffect"), gameObject.transform);
                dashEffect.GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
                Destroy(dashEffect, 0.5f);
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
                if (!normalAttacking && !specialAttacking && Time.time > NAStartTime + PL_Player.Instance.attackInterval)
                {
                    normalAttacking = true;

                    if(grounded)
                    {
                        myRigidbody2D.velocity = Vector3.zero;
                    }
                              
                    NACounter++;
                    NAStartTime = Time.time;
                    myAnimator.SetTrigger("Normal_Attack_" + NACounter.ToString());

                    if (facingRight)
                    {
                        hitEntities = Physics2D.OverlapBoxAll(transform.position + new Vector3(1f, 0.25f), new Vector3(2, 1.5f, 1), 0, attackLayerMask);
                    }
                    else
                    {
                        hitEntities = Physics2D.OverlapBoxAll(transform.position + new Vector3(-1f, 0.25f), new Vector3(2, 1.5f, 1), 0, attackLayerMask);
                    }

                    foreach(Collider2D collider in hitEntities)
                    {
                        var breakable = collider.GetComponent<IBreakable>();
                        if(breakable != null)
                        {
                            collider.gameObject.GetComponent<IBreakable>().BreakObject();
                        }

                        var damageable = collider.GetComponent<IDamageable>();
                        if (damageable != null)
                        {
                            if(NACounter == 3)
                            {
                                Instantiate(Resources.Load<GameObject>("HitEffects/Explosion/Explosion_Hit_Effect"), collider.transform.position, Quaternion.Euler(0, 0, -45 * facingDir.z));
                            }
                            else
                            {
                                Instantiate(Resources.Load<GameObject>("HitEffects/MeleeHit/Slash_Effect_Fire"), collider.transform.position, Quaternion.identity);
                            }
                            collider.gameObject.GetComponent<IDamageable>().InflictDamage(PL_Player.Instance.normalAttackDamage);
                        }
                    }
                }
            }
        }
    }

    public void SpecialAttack(InputAction.CallbackContext context)
    {
        if (allowInput)
        {
            if (context.performed)
            {
                if (!specialAttacking && !normalAttacking && !PL_Player.Instance.specialUsed && Time.time > NAStartTime + PL_Player.Instance.attackInterval)
                {
                    if (grounded)
                    {
                        myRigidbody2D.velocity = Vector3.zero;
                    }

                    myAnimator.SetTrigger("Special_Attack");

                    specialAttacking = true;
                    PL_Player.Instance.specialUsed = true;
                    SAStartTime = Time.time;
                }
            }
        }
    }

    private void AttackCheck()
    {       
        if (normalAttacking && Time.time > NAStartTime + PL_Player.Instance.attackInterval)
        {
            if (NACounter == 3)
            {
                NACounter = 0;
            }
            normalAttacking = false;
        }

        if(specialAttacking && Time.time > SAStartTime + 0.7f)
        {
            GameObject swordWave = Instantiate(swordWavePrefab, transform.position, Quaternion.identity);

            //if (facingRight)
            //{
            //    swordWave.GetComponent<ProjectileBase>().Initialize(Vector3.right, new Vector3(0, 0, 0), transform.position + new Vector3(1, 0.5f, 1), PL_Player.Instance.specialAttackDamage);
            //}
            //else
            //{
            //    swordWave.GetComponent<ProjectileBase>().Initialize(Vector3.left, new Vector3(0, 180, 0), transform.position + new Vector3(-1, 0.5f, 1), PL_Player.Instance.specialAttackDamage);
            //}
            
            specialAttacking = false;
        }
    }

    private void HorizontalMovement()
    {
        if(!dashing && !normalAttacking && !specialAttacking)
        {
            var acceleration = grounded ? moveAcceleration : moveAcceleration * 0.5f;

            if (moveRight)
            {
                if (myRigidbody2D.velocity.x < 0)
                {
                    moveDistance.x = 0;
                }
                moveDistance.x = Mathf.MoveTowards(moveDistance.x, 1, acceleration * Time.deltaTime);
            }
            else if (moveLeft)
            {
                if (myRigidbody2D.velocity.x > 0 && moveLeft)
                {
                    moveDistance.x = 0;
                }
                moveDistance.x = Mathf.MoveTowards(moveDistance.x, -1, acceleration * Time.deltaTime);
            }
            else
            {
                moveDistance.x = Mathf.MoveTowards(moveDistance.x, 0, acceleration * 2 * Time.deltaTime);
            }

            Vector3 targetVelocity = new Vector3(moveDistance.x * moveSpeed, myRigidbody2D.velocity.y);
            myRigidbody2D.velocity = Vector3.MoveTowards(myRigidbody2D.velocity, targetVelocity, 100 * Time.deltaTime);

            myAnimator.SetBool("IsRunning", moveDistance.x != 0 && grounded);
        }      
    }

    private void VerticalMovement()
    {
        if(jumpExecuted && grounded && jumpCount < PL_Player.Instance.jumpCharges)
        {
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, jumpHeight);
            jumpExecuted = false;
            jumping = true;
            jumpCount++;
            myAnimator.SetTrigger("JumpTrigger");
            myAnimator.SetBool("IsJumping", jumping);
        }
        else if (jumpExecuted && coyotePossible && jumpCount < PL_Player.Instance.jumpCharges)
        {
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, jumpHeight);
            jumpExecuted = false;            
            jumping = true;
            jumpCount++;
            if(jumpCount == PL_Player.Instance.jumpCharges)
            {
                coyotePossible = false;
            }    
            myAnimator.SetTrigger("JumpTrigger");
            myAnimator.SetBool("IsJumping", jumping);
        }

        if (!grounded)
        {
            if (myRigidbody2D.velocity.y < jumpFalloff || myRigidbody2D.velocity.y > 0)
            {
                if(!dashing)
                {
                   myRigidbody2D.velocity += fallSpeed * Physics.gravity.y * Vector2.up * Time.deltaTime;
                }              
            }
        }
    }

    private void DashMovement()
    {
        if (dashExecuted)
        {
            dashDir = new Vector3(inputAxis.x, inputAxis.y).normalized;

            if(dashDir == Vector3.zero)
            {
                if(facingRight)
                {
                    dashDir = Vector3.right;
                }
                else 
                {
                    dashDir = Vector3.left;
                }
            }

            dashing = true;
            dashExecuted = false;
            allowInput = false;
            myRigidbody2D.gravityScale = 0;
            dashExecutionTime = Time.time;
        }

        if (dashing)
        {
            myRigidbody2D.velocity = dashDir * dashSpeed;
            CreateAfterimage();
            if (Time.time >= dashExecutionTime + dashDuration)
            {
                dashing = false;
                myRigidbody2D.velocity = new Vector3(myRigidbody2D.velocity.x, myRigidbody2D.velocity.y > 3 ? 3 : myRigidbody2D.velocity.y);
                myRigidbody2D.gravityScale = 1;
                allowInput = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(dashing)
        {
            myRigidbody2D.velocity = Vector3.zero;
        }       
    }

    private void CreateJumpEffect()
    {
        Instantiate(Resources.Load<GameObject>("HitEffects/JumpEffect/Jump_VFX"), transform.position - new Vector3(0, 0.5f, 0), Quaternion.identity);
        SoundManager.Instance.PlaySound(Resources.Load<AudioClip>("SFX/SFX_Jump"));
        jumpDust.Play();
    }

    private void CreateAfterimage()
    {
        GameObject afterimage = Instantiate(Resources.Load<GameObject>("PlayerDash/PlayerAfterimage"), transform.position, Quaternion.identity);
        afterimage.GetComponent<SpriteAfterimage>().Initialize(GetComponent<SpriteRenderer>().sprite, GetComponent<SpriteRenderer>().flipX);
    }

    public void Stagger()
    {

    }

    public void InflictDamage(float damageToInflict)
    {
        PL_Player.Instance.playerHP -= damageToInflict;

        GameObject damageNumber = Instantiate(Resources.Load<GameObject>("DamageNumber"), transform.position, Quaternion.identity);
        damageNumber.GetComponent<FloatingText>().textString = "- " + damageToInflict.ToString();
    }
}
