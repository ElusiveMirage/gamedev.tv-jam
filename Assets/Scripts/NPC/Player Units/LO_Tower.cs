using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LO_Tower : UnitBase
{
    [SerializeField] private float skillCooldown;
    [SerializeField] private float lastAttackTime = 0f;
    [SerializeField] private Vector3 attackDir;
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
        attackDir = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        unitHPBar.SetMaxValue(maxHP);
        unitHPBar.SetMinValue(HP);

        skillButton.image.fillAmount += Time.deltaTime / skillCooldown;

        if(skillButton.image.fillAmount == 1f)
        {
            skillNotification.SetActive(true);
        }
        else 
        {
            skillNotification.SetActive(false);
        }

        if(skillNotification.activeSelf && canvasButtons.activeSelf)
        {
            skillNotification.SetActive(false);
        }

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {

    }

    public void UseSkill()
    {
        if(skillButton.image.fillAmount != 1f)
        {
            return;
        }

        if(attackDir == Vector3.up)
        {
            enemiesInRange = Physics2D.OverlapBoxAll(transform.position + new Vector3(0, 4, 0), new Vector3(2, 9, 1), 0, attackLayerMask);
        }
        else if(attackDir == Vector3.down)
        {
            enemiesInRange = Physics2D.OverlapBoxAll(transform.position + new Vector3(0, -4, 0), new Vector3(2, 9, 1), 0, attackLayerMask);
        }
        else if (attackDir == Vector3.right)
        {
            enemiesInRange = Physics2D.OverlapBoxAll(transform.position + new Vector3(4, 0, 0), new Vector3(9, 2, 1), 0, attackLayerMask);
        }
        else if (attackDir == Vector3.left)
        {
            enemiesInRange = Physics2D.OverlapBoxAll(transform.position + new Vector3(-4, 0, 0), new Vector3(9, 2, 1), 0, attackLayerMask);
        }

        if (enemiesInRange.Length > 0)
        {
            if (Time.time > lastAttackTime + skillCooldown)
            {
                lastAttackTime = Time.time;

                foreach (Collider2D potentialTarget in enemiesInRange)
                {
                    potentialTarget.GetComponent<IDamageable>().InflictDamage(ATK);
                    Instantiate(hitPrefab, potentialTarget.transform);
                }

                GameObject attack = Instantiate(attackPrefab, transform);
                SoundManager.Instance.PlaySound(Resources.Load<AudioClip>("SFX/SFX_Explosion"));

                if (attackDir == Vector3.up)
                {
                    attack.transform.eulerAngles = new Vector3(0, 0, 90);
                }
                else if (attackDir == Vector3.down)
                {
                    attack.transform.eulerAngles = new Vector3(0, 0, 270);
                }
                else if (attackDir == Vector3.right)
                {
                    attack.transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else if (attackDir == Vector3.left)
                {
                    attack.transform.eulerAngles = new Vector3(0, 0, 180);
                }
            }

            skillButton.image.fillAmount = 0f;

            UnselectUnit();
        }
    }

    public void ShiftUp()
    {
        rangePrefab.transform.eulerAngles = new Vector3(0, 0, 0);
        attackDir = Vector3.up;
    }
    public void ShiftDown()
    {
        rangePrefab.transform.eulerAngles = new Vector3(0, 0, 180);
        attackDir = Vector3.down;
    }
    public void ShiftRight()
    {
        rangePrefab.transform.eulerAngles = new Vector3(0, 0, 270);
        attackDir = Vector3.right;
    }
    public void ShiftLeft()
    {
        rangePrefab.transform.eulerAngles = new Vector3(0, 0, 90);
        attackDir = Vector3.left;
    }

    private void OnDrawGizmos()
    {
        DrawAttackRangeGizmo();
    }

    private void DrawAttackRangeGizmo()
    {
        Gizmos.color = Color.red;

        if (attackDir == Vector3.up)
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(0, 4, 0), new Vector3(2, 9, 0));
        }
        else if (attackDir == Vector3.down)
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(0, -4, 0), new Vector3(2, 9, 0));
        }
        else if (attackDir == Vector3.right)
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(4, 0, 0), new Vector3(9, 2, 0));
        }
        else if (attackDir == Vector3.left)
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(-4, 0, 0), new Vector3(9, 2, 0));
        }
    }
}
