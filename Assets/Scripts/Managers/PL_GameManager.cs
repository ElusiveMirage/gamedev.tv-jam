using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PL_GameManager : MonoBehaviour
{
    //=============================================//
    [SerializeField] public bool gamePaused;
    [SerializeField] public bool gameOver;
    //=============================================//
    [SerializeField] private PL_Player playerData;
    [SerializeField] private GameObject playerObject;
    //=============================================//
    [Header("UI Elements")]
    [SerializeField] private UI_Bar playerHPBar;
    [SerializeField] private UI_Bar playerSPBar;
    [SerializeField] private GameObject potionBar;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject gameOverScreen;
    //=============================================//
    [Header("UI Prefabs")]
    [SerializeField] private GameObject potionIcon;
    //=============================================//

    //==================================================//
    private static PL_GameManager _instance;

    public static PL_GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePaused || gameOver)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        if (!gamePaused)
        {
            UpdateUI();
        }

        if (playerData.playerHP <= 0)
        {
            Destroy(playerObject);
            Invoke("GameOver", 2f);
        }
    }

    private void UpdateUI()
    {
        playerHPBar.SetMaxValue(playerData.playerMaxHP);
        playerHPBar.SetMinValue(playerData.playerHP);
        playerSPBar.SetMaxValue(playerData.playerMaxSP);
        playerSPBar.SetMinValue(playerData.playerSP);
        goldText.text = playerData.goldAmount.ToString();
    }

    public void UpdatePotionbar(bool modifier)
    {
        if(modifier)
        {
            Instantiate(potionIcon, potionBar.transform);
        }
        else
        {
            Destroy(potionBar.transform.GetChild(potionBar.transform.childCount - 1));
        }
    }

    private void GameOver()
    {
        gameOver = true;
        gameOverScreen.SetActive(true);
    }

    public void PauseGame()
    {
        if (gamePaused)
        {
            SoundManager.Instance.PlaySound(Resources.Load<AudioClip>("SFX/SFX_UI_Resume"));
        }
        else
        {
            SoundManager.Instance.PlaySound(Resources.Load<AudioClip>("SFX/SFX_UI_Pause"));
        }

        gamePaused = !gamePaused;
        pauseScreen.SetActive(gamePaused);
    }

    public void Return()
    {
        Destroy(gameObject.transform.parent.gameObject);
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene("MainMenuScene");
    }
}
