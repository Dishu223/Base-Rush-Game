using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References (Assign in Inspector)")]
    public Text coinsText;
    public Text armyUpgradeText;
    public Text incomeUpgradeText;

    private int totalCoins;
    private int armyUpgradeLevel;
    private int incomeUpgradeLevel;

    private int armyUpgradeCost = 25;
    private int incomeUpgradeCost = 50;

    void Start()
    {
        // Make sure time flows normally in the menu
        Time.timeScale = 1f;

        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        armyUpgradeLevel = PlayerPrefs.GetInt("StartingArmyUpgrade", 0);
        incomeUpgradeLevel = PlayerPrefs.GetInt("IncomeMultiplierUpgrade", 1);
        UpdateUI();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void PlayGame()
    {
        // Assumes your game scene is named "SampleScene" or is build index 1
        // We will just load Scene 1
        SceneManager.LoadScene(1);
    }

    public void BuyArmyUpgrade()
    {
        int cost = armyUpgradeCost + (armyUpgradeLevel * 10);
        if (totalCoins >= cost)
        {
            totalCoins -= cost;
            armyUpgradeLevel++;
            
            PlayerPrefs.SetInt("TotalCoins", totalCoins);
            PlayerPrefs.SetInt("StartingArmyUpgrade", armyUpgradeLevel);
            UpdateUI();
        }
    }

    public void BuyIncomeUpgrade()
    {
        int cost = incomeUpgradeCost + ((incomeUpgradeLevel - 1) * 25);
        if (totalCoins >= cost)
        {
            totalCoins -= cost;
            incomeUpgradeLevel++;
            
            PlayerPrefs.SetInt("TotalCoins", totalCoins);
            PlayerPrefs.SetInt("IncomeMultiplierUpgrade", incomeUpgradeLevel);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (coinsText != null) coinsText.text = "COINS: " + totalCoins;
        
        int aCost = armyUpgradeCost + (armyUpgradeLevel * 10);
        if (armyUpgradeText != null) armyUpgradeText.text = "START ARMY (+" + armyUpgradeLevel + ")\nCost: " + aCost;
        
        int iCost = incomeUpgradeCost + ((incomeUpgradeLevel - 1) * 25);
        if (incomeUpgradeText != null) incomeUpgradeText.text = "COIN VALUE (x" + incomeUpgradeLevel + ")\nCost: " + iCost;
    }
}
