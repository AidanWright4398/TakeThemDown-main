using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [SerializeField]  GameState state;
    [SerializeField] PlayerMove playerMove;
    [SerializeField] PlayerScript playerScript;
    [SerializeField] int playerHealth = 10;

    [SerializeField] UiManager uiManager = new UiManager();
    [SerializeField] VolumeSettings volSetting = new VolumeSettings();

    private float currentHealth;
    private int enemyHit, shotsFired, enemyKilled, totalEnemy, hostageKilled;

    private TimerObject timerObject = new TimerObject();

    public bool GamePaused { get; private set; } 
    public bool PlayerDead { get; private set; }
    private void Awake()
    {
        Instance = this;
    }



    // Start is called before the first frame update
    void Start()
    {
        SwitchState(GameState.Start);
        Init();
    }

    void Init()
    {
        currentHealth = playerHealth;
        uiManager.Init(currentHealth);
        volSetting.Init();
        PlayerMove.OnLevelFinished += ShowEndScreen;
    }
    private void OnDisable()
    {
        PlayerMove.OnLevelFinished -= ShowEndScreen;
        uiManager.RemoveEvent(); 
    }


    public void SwitchState(GameState newState)
    {
      if (state == newState) 
            return;

        state = newState;
        switch(state)
        {
            case GameState.Start:
                Debug.Log("Game Start");
                playerMove.enabled = false;
                this.DelayedAction(delegate { SwitchState(GameState.Gameplay); }, 3f);
                break;
            case GameState.Gameplay:
                Debug.Log("State: Gameplay " + Time.time);
                playerMove.enabled = true;
                break;
            case GameState.LevelEnd:
                break;
        }
    }

    public void ShotHit()
    {
        
            enemyHit++;

       
    }

    public void ShotsFired()
    {
        shotsFired++;
    }

    public void PlayerHit(float damage)
    {
        currentHealth -= damage;
        uiManager.UpdateHealth(currentHealth);
        playerScript.ShakeCamera(0.5f, 0.2f, 5f);

        if(currentHealth <=  0f)
        {
            ShowGameOverScreen();
            PlayerDead = true;
        }
    }

   /* public void StartTimer(float duration)
    {
        timerObject.StartTimer(this, duration);
    }*/

    /*public void StopTimer()
    {
        timerObject.StopTimer(this);
    }*/

    public void RegisterEnemy()
    {
        totalEnemy++;
    }

    public void HostageKilled(Vector3 worldPos)
    {
        hostageKilled++;
        ShowHostageKilled(worldPos, true);
        this.DelayedAction(delegate { ShowHostageKilled(worldPos, false); }, 3f);
    }

    public void EnemyKilled()
    {
        enemyKilled++;
    }

    void ShowHostageKilled(Vector3 pos, bool show)
    {
        Vector3 screenPos = playerMove.GetComponent<Camera>().WorldToScreenPoint(pos);
        uiManager.ShowHostageKilled(screenPos, show);
    }
void ShowEndScreen()
    {
        this.DelayedAction(delegate { uiManager.ShowEndScreen(enemyKilled, totalEnemy, hostageKilled, shotsFired, enemyHit); }, 0.2f);
        
    }

    void ShowGameOverScreen()
    {
        this.DelayedAction(delegate { uiManager.ShowGameOverScreen(enemyKilled, totalEnemy, hostageKilled, shotsFired, enemyHit); }, 0.2f);

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = Time.timeScale == 0f ? 1f : 0f;

            volSetting.Panel.SetActive(Time.timeScale == 0f);

            GamePaused = Time.timeScale == 0f; ;

        }
        uiManager.MoveCrosshair(Input.mousePosition);
    }
}


public enum GameState
{
    Default,
    Start,
    Gameplay,
    LevelEnd,
    NextLevel,
    
}