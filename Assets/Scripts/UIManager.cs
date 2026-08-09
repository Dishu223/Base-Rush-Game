using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Elements")]
    public TextMeshProUGUI unitCountText;
    public TextMeshProUGUI bossHealthText;
    
    public GameObject winPanel;
    public GameObject losePanel;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Hide panels at the start
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    public void UpdateBossHealth(int health)
    {
        if (bossHealthText != null)
        {
            bossHealthText.text = "Boss Health: " + health;
        }
    }

    public void UpdateUnitCount(int count)
    {
        if (unitCountText != null)
        {
            unitCountText.text = "Units: " + count;
        }
    }

    public void ShowWinScreen()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        if (losePanel != null) losePanel.SetActive(true);
    }
}
