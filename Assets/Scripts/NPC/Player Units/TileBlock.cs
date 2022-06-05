using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBlock : UnitBase
{
    // Start is called before the first frame update
    void Start()
    {
        HP = maxHP;
        SP = 0;
        BlockTile();
    }

    // Update is called once per frame
    void Update()
    {
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

    private void BlockTile()
    {

    }

    private void UnblockTile()
    {

    }
}
