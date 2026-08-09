using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI upgradeArmyText;
    public TextMeshProUGUI upgradeIncomeText;

    [Header("Shop Settings")]
    public int armyUpgradeCost = 25;
    public int incomeUpgradeCost = 50;
    
    private int totalCoins;
    private int armyUpgradeLevel;
    private int incomeUpgradeLevel;

    void Start()
    {
        // Ensure time flows normally in the menu in case we quit during Slow Mo
        Time.timeScale = 1f;

        // Load saved data using the exact keys from GameManager
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        armyUpgradeLevel = PlayerPrefs.GetInt("StartingArmyUpgrade", 0);
        incomeUpgradeLevel = PlayerPrefs.GetInt("IncomeMultiplierUpgrade", 1);

        UpdateUI();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    private void UpdateUI()
    {
        if (coinText != null) coinText.text = "COINS: " + totalCoins;

        int aCost = armyUpgradeCost + (armyUpgradeLevel * 10);
        if (upgradeArmyText != null) upgradeArmyText.text = "START ARMY (+" + armyUpgradeLevel + ")\nCost: " + aCost;

        int iCost = incomeUpgradeCost + ((incomeUpgradeLevel - 1) * 25);
        if (upgradeIncomeText != null) upgradeIncomeText.text = "COIN VALUE (x" + incomeUpgradeLevel + ")\nCost: " + iCost;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); 
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
}
