using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI upgradeArmyText;

    [Header("Shop Settings")]
    public int armyUpgradeCost = 50;
    
    private int totalCoins;
    private int startingArmySize;

    void Start()
    {
        // Load saved data
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        startingArmySize = PlayerPrefs.GetInt("StartingArmySize", 4); // Default to 4 units

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + totalCoins;
        }

        if (upgradeArmyText != null)
        {
            upgradeArmyText.text = "Upgrade Army: " + armyUpgradeCost + " coins\n(Current: " + startingArmySize + ")";
        }
    }

    public void PlayGame()
    {
        // Assuming your main game scene is at build index 1, or named "SampleScene"
        // Update this to match your exact gameplay scene name!
        SceneManager.LoadScene("SampleScene"); 
    }

    public void BuyArmyUpgrade()
    {
        if (totalCoins >= armyUpgradeCost)
        {
            totalCoins -= armyUpgradeCost;
            startingArmySize += 1;

            // Save new values
            PlayerPrefs.SetInt("TotalCoins", totalCoins);
            PlayerPrefs.SetInt("StartingArmySize", startingArmySize);

            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }
}
