using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] private float HP;
    [SerializeField] private float maxHP;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float damage;
    [SerializeField] private int scoreValue;
    //================================================//
    [SerializeField] private int currentNode;
    [SerializeField] private int currentWaypoint;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private UI_Bar HPBar;
    [SerializeField] List<Transform> waypointRoute = new List<Transform>();
    [SerializeField] List<Node2D> aStarPath = new List<Node2D>();
    //================================================//
    [SerializeField] private float attackStartTime = 0f;
    [SerializeField] private bool attacked = false;
    //================================================//
    private bool isAlive;
    private float originalSpeed;
    private GameObject player;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();

        maxHP = maxHP * GameManager.Instance.enemyStatMult;
        moveSpeed = moveSpeed * GameManager.Instance.enemyStatMult;
        damage = damage * GameManager.Instance.enemyStatMult;
        scoreValue = scoreValue * (int)GameManager.Instance.enemyStatMult;

        HP = maxHP;

        originalSpeed = moveSpeed;
        isAlive = true;

        player = GameManager.Instance.playerCharacter;
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive && !GameManager.Instance.playerDied)
        {
            HPBar.SetMaxValue(maxHP);
            HPBar.SetMinValue(HP);

            if (HP > 0)
            {
                if (waypointRoute.Count > 0 && currentWaypoint < waypointRoute.Count)
                {
                    if (aStarPath.Count > 0 && currentNode < aStarPath.Count)
                    {
                        GridMovement();
                    }
                }
            }
            else
            {
                Death();
                Instantiate(deathEffect, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                Destroy(gameObject, 1f);
                isAlive = false;
            }
        }
        
        if(!isAlive)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(GetComponent<SpriteRenderer>().color.a, 0f, Time.deltaTime));
        }      
    }

    private void FixedUpdate()
    {
        if(!GameManager.Instance.playerDied)
        {
            if(isAlive)
            {
                //Movement
                Vector3 direction = GameManager.Instance.playerCharacter.transform.position - transform.position;

                direction.Normalize();

                float distance = Vector3.Distance(GameManager.Instance.playerCharacter.transform.position, transform.position);

                if (distance > 1)
                {
                    myRigidbody.MovePosition(transform.position + (moveSpeed * Time.deltaTime * direction));
                }

                if (GameManager.Instance.playerCharacter.transform.position.x < transform.position.x)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }

                //Melee Attack
                if (distance < 2f)
                {
                    if (Time.time > attackStartTime + 2f)
                    {
                        myAnimator.SetTrigger("Attack");

                        attackStartTime = Time.time;
                        attacked = true;
                    }

                    if (attacked && Time.time > attackStartTime + 0.5f)
                    {
                        GameManager.Instance.playerCharacter.GetComponent<PlayerController>().InflictDamage(damage);
                        attacked = false;
                    }
                }
            }
        }      
    }

    private void GetAstarPath(Transform targetTransform)
    {
        if (GetComponent<AStarPathfinding>())
        {
            aStarPath = GetComponent<AStarPathfinding>().CalculatePath(transform.position, targetTransform.transform.position);
        }
    }

    private void GridMovement()
    {
        if(currentNode < aStarPath.Count)
        {
            if (transform.position != aStarPath[currentNode].worldPos && !aStarPath[currentNode].unitPlaced)
            {
                transform.position = Vector3.MoveTowards(transform.position, aStarPath[currentNode].worldPos, moveSpeed * Time.deltaTime);
            }
            else
            {
                currentNode++;
            }
        }

        if(currentNode == aStarPath.Count)
        {
            currentWaypoint++;

            if (currentWaypoint < waypointRoute.Count)
            {             
                GetAstarPath(waypointRoute[currentWaypoint]);
                currentNode = 0;
            }
        }
    }

    public void Initialize(List<Transform> route, NodeGrid2D grid)
    {
        waypointRoute = route;
        GetComponent<AStarPathfinding>().nodeGrid = grid;
    }

    public void InflictDamage(float damageToInflict)
    {
        HP -= damageToInflict;
        GameObject damageNumber = Instantiate(Resources.Load<GameObject>("DamageNumber"), transform.position, Quaternion.identity);
        damageNumber.GetComponent<FloatingText>().textString = "- " + damageToInflict.ToString();
    }

    public void ModifyMoveSpeed(float modifier)
    {
        moveSpeed = originalSpeed * modifier;
    }

    public void RestoreMoveSpeed()
    {
        moveSpeed = originalSpeed;
    }

    private void Death()
    {
        GameManager.Instance.stageScore += scoreValue;
        GameManager.Instance.enemyCount--;
    }
}
