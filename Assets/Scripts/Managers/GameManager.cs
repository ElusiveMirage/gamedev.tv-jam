using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerData playerData;
    public GameObject playerCharacter;
    //=============================================//
    public bool gamePaused;
    public bool gameOver;
    public bool stageComplete;
    public bool playerDied;
    public bool mortal;
    //=============================================//
    [Header("UI Elements")]
    [SerializeField] private UI_Bar playerHPBar;
    [SerializeField] private UI_Bar playerMPBar;
    [SerializeField] private GameObject playerLives;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject healthUpgrade;
    [SerializeField] private GameObject attackUpgrade;
    [SerializeField] private GameObject counterUpgrade;
    [SerializeField] private GameObject moveSpeedUpgrade;
    [SerializeField] public GameObject counterCD_UI;
    [SerializeField] private Image counterCDFill;
    [SerializeField] private TextMeshProUGUI counterCDText;
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI maxHPText;
    [SerializeField] private TextMeshProUGUI attackLVText;
    [SerializeField] private TextMeshProUGUI counterLVText;
    [SerializeField] private TextMeshProUGUI healthLVText;
    [SerializeField] private TextMeshProUGUI moveSpeedLVText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI waveComplete;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private Sprite filledHeart;
    [SerializeField] private Sprite emptyHeart;
    //=============================================//
    [Header("Stage")]
    //==================================================//
    public bool waveStarted;
    public int stageScore;
    public int enemyCount;
    public int waveCount;
    public float counterCDTime;
    public float waveTimer;
    public float enemyStatMult;
    public NodeGrid2D levelGrid;
    public AudioClip stageBGM;
    //==================================================//
    //[Header("Upgrades")]
    //==================================================//
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

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
        GameStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePaused || gameOver || stageComplete)
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

        if (playerData.HP <= 0 && mortal && !playerDied && playerCharacter != null)
        {
            playerCharacter.GetComponent<PlayerController>().Death();
            Invoke("GameOver", 2.5f);
            playerDied = true;
        }
        else if(playerData.HP <= 0 && !playerDied && !mortal)
        {
            playerData.lives--;

            if(playerData.lives == 0)
            {
                mortal = true;
            }
            
            playerCharacter.GetComponent<PlayerController>().Death();
            Invoke("Death", 2.5f);
            playerDied = true;
        }

        WaveCheck();
    }

    private void GameStart()
    {
        waveCount = 1;
        playerDied = false;
        StartWave(15, 2f);
        enemyStatMult = 1f;
        SoundManager.Instance.PlayBGM(stageBGM);
    }

    private void WaveCheck()
    {
        if(waveStarted)
        {
            if (enemyCount <= 0)
            {
                SpawnManager.startSpawn = false;
                waveStarted = false;
                waveComplete.gameObject.SetActive(true);
                waveComplete.text = "WAVE " + waveCount.ToString() + " COMPLETE!";
                Invoke("StartNextWave", 2f);
            }
        }        
    }

    private void StartWave(int enemies, float spawn)
    {
        enemyCount = enemies;
        waveStarted = true;
        SpawnManager.spawnInterval = spawn;
        SpawnManager.startSpawn = true;
        SpawnManager.enemiesSpawned = 0;
        SpawnManager.enemiesThisWave = enemies;
    }

    private void Death()
    {
        gamePaused = true;
        playerDied = true;
        deathScreen.SetActive(true);
    }

    public void UpgradeHP()
    {
        if(playerData.healthLV < 4)
        {
            playerData.healthLV++;
            playerData.StatsCheck();
            playerData.HP = playerData.maxHP;
            if(playerData.healthLV == 4)
            {
                healthUpgrade.SetActive(false);
            }
        }

        EndDeath();
    }

    public void UpgradeMoveSpeed()
    {
        if (playerData.moveSpeedLV < 4)
        {
            playerData.moveSpeedLV++;

            if (playerData.moveSpeedLV == 4)
            {
                moveSpeedUpgrade.SetActive(false);
            }
        }

        EndDeath();
    }

    public void UpgradeAttack()
    {
        if (playerData.attackLV < 4)
        {
            playerData.attackLV++;

            if (playerData.attackLV == 4)
            {
                attackUpgrade.SetActive(false);
            }
        }

        EndDeath();
    }

    public void UpgradeCounter()
    {
        if (playerData.counterLV < 4)
        {
            playerData.counterLV++;

            if (playerData.counterLV == 4)
            {
                counterUpgrade.SetActive(false);
            }
        }

        EndDeath();
    }

    public void EndDeath()
    {
        playerData.HP = playerData.maxHP;
        playerDied = false;
        deathScreen.SetActive(false);
        playerCharacter.GetComponent<PlayerController>().Resurrection();
        gamePaused = false;
    }

    private void UpdateUI()
    {
        //Player stats
        playerHPBar.SetMaxValue(playerData.maxHP);
        playerHPBar.SetMinValue(playerData.HP);
        HPText.text = playerData.HP.ToString();
        maxHPText.text = playerData.maxHP.ToString();

        attackLVText.text = playerData.attackLV.ToString();
        counterLVText.text = playerData.counterLV.ToString();
        healthLVText.text = playerData.healthLV.ToString();
        moveSpeedLVText.text = playerData.moveSpeedLV.ToString();

        waveText.text = waveCount.ToString() + "]";
        
        scoreText.text = stageScore.ToString();
        enemiesText.text = ((int)enemyCount).ToString();      

        for (int i = 0; i < playerLives.transform.childCount; i++)
        {
            if(i < playerData.lives)
            {
                playerLives.transform.GetChild(i).GetComponent<Image>().sprite = filledHeart;
            }
            else
            {
                playerLives.transform.GetChild(i).GetComponent<Image>().sprite = emptyHeart;
            }           
        }

        if(counterCDTime > 0)
        {
            if (counterCDFill.fillAmount < 1f)
            {
                counterCDFill.fillAmount += 1f * Time.deltaTime;
            }
            else
            {
                counterCDFill.fillAmount = 0f;
            }

            counterCDTime -= 1f * Time.deltaTime;
            counterCDText.text = ((int)counterCDTime).ToString();
        }
        else
        {
            counterCD_UI.SetActive(false);
        }
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
        SoundManager.Instance.StopBGM();
        Destroy(gameObject.transform.parent.gameObject);
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene("MainMenuScene");
    }

    public void Retry()
    {
        Destroy(gameObject.transform.parent.gameObject);
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void StartNextWave()
    {
        waveComplete.gameObject.SetActive(false);
        waveCount++;

        if(waveCount < 3)
        {
            StartWave(15 + waveCount * 2, 1f);
        }
        else if (waveCount >= 3 && waveCount <= 6)
        {
            StartWave(25 + waveCount * 3, 1f);
            enemyStatMult = 1.5f;
        }
        else if(waveCount > 6)
        {
            StartWave(30 + waveCount * 4, 1f);
            enemyStatMult = 2f;
        }
    }

    private void GameOver()
    {
        SoundManager.Instance.StopBGM();
        gameOver = true;
        gameOverScreen.SetActive(true);
    }

    //private void StageComplete()
    //{
    //    SoundManager.Instance.StopBGM();
    //    stageComplete = true;
    //    victoryScreen.SetActive(true);
    //}
}
