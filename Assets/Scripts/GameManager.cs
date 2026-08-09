using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    public int enemyBaseArmySize = 25; // How many units you need to win
    public bool isGameOver = false;
    public bool isBossPhase = false;

    [Header("Progression")]
    public int currentLevel = 1;
    public int startingArmyUpgrade = 0;
    public int incomeMultiplierUpgrade = 1;

    [Header("Economy")]
    public int coins = 0;

    void Awake()
    {
        // Simple Singleton pattern so we can access GameManager from anywhere
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Load saved data
        coins = PlayerPrefs.GetInt("TotalCoins", 0);
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        startingArmyUpgrade = PlayerPrefs.GetInt("StartingArmyUpgrade", 0);
        incomeMultiplierUpgrade = PlayerPrefs.GetInt("IncomeMultiplierUpgrade", 1);
        
        // Ensure time is normal when scene reloads!
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void AddCoins(int amount)
    {
        coins += (amount * incomeMultiplierUpgrade);
        PlayerPrefs.SetInt("TotalCoins", coins); // Save automatically
    }

    public void NextLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float GetLevelSpeedModifier()
    {
        // Increase speed by 5% per level, clamped at 2.5x max speed
        return Mathf.Clamp(1f + ((currentLevel - 1) * 0.05f), 1f, 2.5f);
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
        StartCoroutine(WinSequence());
    }

    private System.Collections.IEnumerator WinSequence()
    {
        // Trigger Epic Slow Motion!
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Wait in REAL-TIME because game time is slowed down (Extended to 5 seconds!)
        yield return new WaitForSecondsRealtime(5.0f); 
        
        // Stop the background completely!
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0.02f;

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
