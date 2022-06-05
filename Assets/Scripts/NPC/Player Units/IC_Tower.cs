using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IC_Tower : UnitBase
{
    [SerializeField] private float skillCooldown;
    [SerializeField] private float skillDuration;
    [SerializeField] private float attackInterval;
    [SerializeField] private float activationTime = 0f;
    [SerializeField] private float lastAttackTime = 0f;
    [SerializeField] private bool skillActive = false;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private GameObject skillNotification;
    [SerializeField] private Button skillButton;
    [SerializeField] private LayerMask attackLayerMask;
    [SerializeField] private Collider2D[] enemiesInRange = new Collider2D[0];

    // Start is called before the first frame update
    void Start()
    {
        HP = maxHP;
        SP = 0;
        skillButton.image.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        unitHPBar.SetMaxValue(maxHP);
        unitHPBar.SetMinValue(HP);

        skillButton.image.fillAmount += Time.deltaTime / skillCooldown;

        if (skillButton.image.fillAmount == 1f)
        {
            skillNotification.SetActive(true);
        }
        else
        {
            skillNotification.SetActive(false);
        }

        if (skillNotification.activeSelf && canvasButtons.activeSelf)
        {
            skillNotification.SetActive(false);
        }

        if(skillActive)
        {
            IceField();
        }

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {

    }

    private void IceField()
    {
        if (Time.time < activationTime + skillDuration)
        {
            enemiesInRange = Physics2D.OverlapBoxAll(transform.position + new Vector3(0, 0, 0), new Vector3(8, 8, 1), 0, attackLayerMask);

            if (enemiesInRange.Length > 0)
            {
                if (Time.time > lastAttackTime + attackInterval)
                {
                    lastAttackTime = Time.time;

                    foreach (Collider2D potentialTarget in enemiesInRange)
                    {
                        potentialTarget.GetComponent<IDamageable>().InflictDamage(ATK);
                        Instantiate(hitPrefab, potentialTarget.transform);
                    }
                }
            }
        }
        else
        {
            enemiesInRange = Physics2D.OverlapBoxAll(transform.position + new Vector3(0, 0, 0), new Vector3(8, 8, 1), 0, attackLayerMask);

            foreach (Collider2D potentialTarget in enemiesInRange)
            {
                potentialTarget.GetComponent<EnemyBase>().RestoreMoveSpeed();
            }

            skillActive = false;
            attackPrefab.SetActive(false);
        }       
    }

    public void UseSkill()
    {
        if (skillButton.image.fillAmount != 1f)
        {
            return;
        }

        skillActive = true;
        attackPrefab.SetActive(true);
        activationTime = Time.time;

        skillButton.image.fillAmount = 0f;

        UnselectUnit();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (skillActive)
        {
            if (collision.gameObject.tag == "TD_Enemy")
            {
                collision.gameObject.GetComponent<EnemyBase>().ModifyMoveSpeed(0.6f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "TD_Enemy")
        {
            collision.gameObject.GetComponent<EnemyBase>().RestoreMoveSpeed();
        }
    }

    private void OnDrawGizmos()
    {
        DrawAttackRangeGizmo();
    }

    private void DrawAttackRangeGizmo()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(8, 8, 0));
    }
}
