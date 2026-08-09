using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    public int enemyBaseArmySize = 25; // How many units you need to win
    public bool isGameOver = false;
    public bool isBossPhase = false;

    [Header("Economy")]
    public int coins = 0;

    void Awake()
    {
        // Simple Singleton pattern so we can access GameManager from anywhere
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Load saved coins
        coins = PlayerPrefs.GetInt("TotalCoins", 0);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("TotalCoins", coins); // Save automatically
    }

    public void StartBossPhase()
    {
        isBossPhase = true;
        // We no longer instantly win/lose at the finish line! 
        // The player controller will stop, and the crowd will swarm the boss.
    }

    public void BossDefeated()
    {
        isGameOver = true;
        if (UIManager.instance != null) UIManager.instance.ShowWinScreen();
    }

    public void FinishLineReached(CrowdManager playerCrowd)
    {
        // This is the old instant win logic. We will replace this call in FinishLine.cs
        // to call StartBossPhase() instead.
    }

    public void GameOver()
    {
        isGameOver = true;
        if (UIManager.instance != null) UIManager.instance.ShowLoseScreen();
    }

    private void GameWon()
    {
        Debug.Log("YOU WIN! Your army conquered the base.");
        if (UIManager.instance != null) UIManager.instance.ShowWinScreen();
    }

    public void GameLost()
    {
        Debug.Log("YOU LOSE! You didn't have enough units.");
        if (UIManager.instance != null) UIManager.instance.ShowLoseScreen();
    }
}
