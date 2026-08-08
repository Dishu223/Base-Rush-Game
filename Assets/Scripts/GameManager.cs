using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    public int enemyBaseArmySize = 25; // How many units you need to win
    public bool isGameOver = false;

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
        
        // Update UI if we have a coin text (we will add this to UIManager next!)
        // if (UIManager.instance != null) UIManager.instance.UpdateCoinText(coins);
    }

    public void FinishLineReached(CrowdManager playerCrowd)
    {
        if (isGameOver) return;
        isGameOver = true;

        int finalPlayerCount = playerCrowd.units.Count;
        Debug.Log("Reached the base with " + finalPlayerCount + " units!");

        if (finalPlayerCount >= enemyBaseArmySize)
        {
            GameWon();
        }
        else
        {
            GameLost();
        }
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
