using UnityEngine;
using UnityEngine.UI;
using System;

public class Hachisha_GameManager : MonoBehaviour
{
    public static Hachisha_GameManager Instance;

    [Header("Time System")]
    public float gameMinutesPerRealSecond = 1f; // 1秒でゲーム内1分進む設定
    public int startHour = 22;
    public int endHour = 6;
    
    private float currentHour;
    private float currentMinute;
    private bool isGameOver = false;

    [Header("UI")]
    public Text timeDisplay; // 時刻表示用UIテキスト

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHour = startHour;
        currentMinute = 0;
    }

    void Update()
    {
        if (isGameOver) return;

        UpdateGameTime();
        CheckWinCondition();
    }

    void UpdateGameTime()
    {
        currentMinute += Time.deltaTime * gameMinutesPerRealSecond;
        if (currentMinute >= 60)
        {
            currentMinute = 0;
            currentHour++;
            if (currentHour >= 24) currentHour = 0;
        }

        if (timeDisplay != null)
        {
            timeDisplay.text = string.Format("{0:00}:{1:00}", (int)currentHour, (int)currentMinute);
        }
    }

    void CheckWinCondition()
    {
        if ((int)currentHour == endHour && !isGameOver)
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        isGameOver = true;
        Debug.Log("Survival Success! 06:00 reached.");
        // TODO: UI表示やシーン遷移
    }

    public void LoseGame(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("Game Over: " + reason);
        // TODO: UI表示やリスタート処理
    }

    public bool IsNight()
    {
        // 22:00〜06:00の間か判定
        if (currentHour >= startHour || currentHour < endHour) return true;
        return false;
    }
}
