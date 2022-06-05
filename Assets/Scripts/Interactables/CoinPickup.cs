using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int goldValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PL_Player.Instance.goldAmount += goldValue;

            GameObject goldNumber = Instantiate(Resources.Load<GameObject>("GoldNumber"), transform.position, Quaternion.identity);
            goldNumber.GetComponent<FloatingText>().textString = "+ " + goldValue.ToString();
            SoundManager.Instance.PlaySound(Resources.Load<AudioClip>("SFX/SFX_Coin"));
            Destroy(gameObject);
        }
    }
}
