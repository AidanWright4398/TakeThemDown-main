using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class UiManager
{
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] RectTransform hostageKilledText;

    [Header("Weapon HUD")]
    [SerializeField] Image weaponIcon;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] GameObject reloadWarning;
    [SerializeField] RectTransform crossHair;

    [Header("Score Properties")]
    [SerializeField] TextMeshProUGUI enemyKilled;
    [SerializeField] TextMeshProUGUI hostageKilled;
    [SerializeField] TextMeshProUGUI shots;
    [SerializeField] TextMeshProUGUI hit;
    [SerializeField] TextMeshProUGUI accuracy;
    [SerializeField] TextMeshProUGUI rankText;
    [SerializeField] GameObject endScreenPanel;
    [SerializeField] GameObject gameoverScreenPanel;
    [SerializeField] Button backButton;
    [SerializeField] Button nextLevelButton;
    [SerializeField] Button retryButton;
    [SerializeField] Button pauseBackButton;
    [SerializeField] Button gameoverBackButton;
    [SerializeField] string titleSceneName;
    [SerializeField] string pauseToMenu;
    [SerializeField] string level2SceneName;
    [SerializeField] string retry;

    private WeaponData currentWeapon;

    public void Init(float maxHealth)
    {
        if (crossHair != null)
            Cursor.visible = false;

        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        hostageKilledText.gameObject.SetActive(false);
        PlayerScript.onWeaponChange += UpdateWeapon;
        TimerObject.OnTimerChanged += UpdateTimer;

        pauseBackButton.onClick.AddListener(GoToTitleScene);
        gameoverBackButton.onClick.AddListener(GoToTitleScene);
        backButton.onClick.AddListener(GoToTitleScene);
        backButton.onClick.AddListener(GoToNextLevel);
        retryButton.onClick.AddListener(Retry);
    }

    private void UpdateTimer(int currentTimer)
    {
        timerText.SetText(currentTimer.ToString("00"));
    }

    public void RemoveEvent()
    {
        PlayerScript.onWeaponChange -= UpdateWeapon;
        //TimerObject.OnTimerChanged -= UpdateTimer;
        currentWeapon.OnWeaponFired -= UpdateAmmo;
    }

    private void UpdateWeapon(WeaponData obj)
    {
        if (currentWeapon != null)
            currentWeapon.OnWeaponFired -= UpdateAmmo;

        currentWeapon = obj;
        currentWeapon.OnWeaponFired += UpdateAmmo;
        weaponIcon.sprite = currentWeapon.GetIcon;

        Debug.Log(obj.name);
    }

    public void UpdateHealth(float value)
    {
        healthBar.value = value;
    }

    void UpdateAmmo(int ammo)
    {
        reloadWarning.SetActive(ammo <= 0);

        ammoText.SetText(ammo.ToString("00"));
    }

    public void ShowHostageKilled(Vector3 pos, bool show)
    {
        hostageKilledText.gameObject.SetActive(show);

        if (!show)
            return;

        hostageKilledText.position = pos;

        Vector2 adjustPos = Extensions.GetPositionInsideScreen(new Vector2(1920f, 1080f), hostageKilledText, 25f);
        hostageKilledText.anchoredPosition = adjustPos;
    }

    public void ShowEndScreen(int enemyKill, int totalEnemy, int hostageKill, int totalShots, int totalHit)
    {
        endScreenPanel.SetActive(true);
        enemyKilled.SetText(((enemyKill / (float)totalEnemy) * 100f).ToString("00") + "%");
        hostageKilled.SetText(hostageKill.ToString());
        shots.SetText(totalShots.ToString());
        hit.SetText(totalHit.ToString());
        accuracy.SetText(((totalHit / (totalShots == 0 ? 1f : (float)totalShots)) * 100f).ToString("00") + "%");

        CalculateScore(enemyKill, totalEnemy, hostageKill, totalShots, totalHit);
    }
    public void ShowGameOverScreen(int enemyKill, int totalEnemy, int hostageKill, int totalShots, int totalHit)
    {
        gameoverScreenPanel.SetActive(true);
        enemyKilled.SetText(((enemyKill / (float)totalEnemy) * 100f).ToString("00") + "%");
        hostageKilled.SetText(hostageKill.ToString());
        shots.SetText(totalShots.ToString());
        hit.SetText(totalHit.ToString());
        accuracy.SetText(((totalHit / (totalShots == 0 ? 1f : (float)totalShots)) * 100f).ToString("00") + "%");

        CalculateScore(enemyKill, totalEnemy, hostageKill, totalShots, totalHit);
    }

    void CalculateScore(int enemyKill, int totalEnemy, int hostageKill, int totalShots, int totalHit)
    {
        //Max Ratio of the Enemy Kill 100 %
        //Max Ratio of Shots Accuracy 100 %
        //Hostage kill will penalized point by 15

        //A - Enemy Kill > 90 % && Accuracy > 80 % = Total Average > 85 %
        //B - Enemy Kill > 75 % && < 90 % &Accuracy > 70 % & < 80 % = Total Average > 72 % & < 85 %
        //C - Enemy Kill > 60 % && < 75 % &Accuracy > 55 % & < 70 % = Total Average > 57 % & < 72 %
        //D - Enemy Kill < 60 % &Accuracy < 55 % = Total Average < 57 %

        float hostagePenalty = hostageKill * 15f;
        float enemyKillRatio = ((enemyKill / (float)totalEnemy) * 100f) - hostagePenalty;
        float accuracyRatio = ((totalHit / (totalShots == 0 ? 1f : (float)totalShots)) * 100f) - hostagePenalty;
        float totalAverage = (enemyKillRatio + accuracyRatio) / 2f;

        if (totalAverage >= 85f)
        {
            rankText.SetText("A");
        }
        else if (totalAverage >= 72f && totalAverage < 85f)
        {
            rankText.SetText("B");
        }
        else if (totalAverage >= 57f && totalAverage < 72f)
        {
            rankText.SetText("C");
        }
        else if (totalAverage == 0f)
        {
            rankText.SetText("E");
        }
        else if (totalAverage < 57f)
        {
            rankText.SetText("D");
        }
    }

    public void MoveCrosshair(Vector3 mousePosition)
    {
        if (crossHair != null)
            crossHair.position = mousePosition;
    }

    void GoToTitleScene()
    {
        SceneManager.LoadScene(titleSceneName);
    }

    void GoToNextLevel()
    {
        SceneManager.LoadScene(level2SceneName);
    }
    void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
