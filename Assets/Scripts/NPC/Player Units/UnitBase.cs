using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour, IDamageable
{
    public float HP;
    public float maxHP;
    public float SP;
    public float maxSP;
    public float SPRegen;
    public float ATK;
    public float MPCost;
    public Node2D gridNode;
    //=========================================================//
    public GameObject rangePrefab;
    public GameObject canvasButtons;
    public UI_Bar unitHPBar;
    //=========================================================//

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InflictDamage(float damageToInflict)
    {
        HP -= damageToInflict;
    }

    public void SelectUnit()
    {
        //if(!TD_GameManager.Instance.unitSelected)
        //{
        //    canvasButtons.SetActive(true);

        //    if(rangePrefab != null)
        //    {
        //        rangePrefab.SetActive(true);
        //    }   
            
        //    TD_GameManager.Instance.unitSelected = true;
        //}
    }

    public void UnselectUnit()
    {
        //canvasButtons.SetActive(false);

        //if (rangePrefab != null)
        //{
        //    rangePrefab.SetActive(true);
        //}

        //TD_GameManager.Instance.unitSelected = false;
    }

    public void RemoveUnit()
    {
        //TD_GameManager.Instance.stageGrid.nodes[gridNode.gridPos.x, gridNode.gridPos.y].unitPlaced = false;
        //TD_GameManager.Instance.playerData.playerMP += MPCost / 3;
        //TD_GameManager.Instance.unitsDeployed--;
        //TD_GameManager.Instance.unitSelected = false;
        //gridNode.deploymentZone.gameObject.SetActive(true);
        //Destroy(gameObject);
    }
}
