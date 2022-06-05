using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private GameObject floatingText;
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
        if (collision.tag == "Player")
        {
            if(PL_Player.Instance.potionCharges < PL_Player.Instance.maxPotionCharges)
            {
                PL_Player.Instance.potionCharges++;

                PL_GameManager.Instance.UpdatePotionbar(true);

                floatingText.SetActive(true);
                floatingText.GetComponent<FloatingText>().textString = "+ Potion";
                gameObject.transform.DetachChildren();
                floatingText.transform.position = transform.position;
                SoundManager.Instance.PlaySound(Resources.Load<AudioClip>("SFX/SFX_1up"));
                Destroy(gameObject);
            }         
        }
    }
}
