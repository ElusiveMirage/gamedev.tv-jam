using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BC_Tower : UnitBase
{
    [SerializeField] private float attackInterval;
    [SerializeField] private float lastAttackTime = 0f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private LayerMask attackLayerMask;
    [SerializeField] private Collider2D[] enemiesInRange = new Collider2D[0];

    // Start is called before the first frame update
    void Start()
    {
        HP = maxHP;
        SP = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Attack();

        unitHPBar.SetMaxValue(maxHP);
        unitHPBar.SetMinValue(HP);

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void Attack()
    {
        enemiesInRange = Physics2D.OverlapBoxAll(transform.position, new Vector3(6, 6, 1), 0, attackLayerMask);

        if (enemiesInRange.Length > 0)
        {
            if(Time.time > lastAttackTime + attackInterval)
            {
                lastAttackTime = Time.time;

                enemiesInRange = Physics2D.OverlapBoxAll(transform.position, new Vector3(6, 6, 1), 0, attackLayerMask);

                Transform target = GetClosestEnemy(enemiesInRange);

                if(target != null)
                {
                    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    //projectile.GetComponent<ProjectileBase>().Initialize((target.transform.position - transform.position), transform.position, ATK);
                }      
            }            
        }
    }

    private Transform GetClosestEnemy(Collider2D[] enemies)
    {
        Transform target = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Collider2D potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                target = potentialTarget.transform;
            }
        }
        return target;
    }

    private void OnDrawGizmos()
    {
        DrawAttackRangeGizmo();
    }

    private void DrawAttackRangeGizmo()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(6, 6, 0));
    }
}
