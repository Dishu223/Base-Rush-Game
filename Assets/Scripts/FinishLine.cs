using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private bool hasFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasFinished) return;

        if (other.CompareTag("Player"))
        {
            hasFinished = true;
            
            if (GameManager.instance != null)
            {
                GameManager.instance.StartBossPhase();
            }

            // Find the boss and tell the crowd to attack it!
            CrowdManager crowd = other.GetComponent<CrowdManager>();
            Boss boss = FindObjectOfType<Boss>();
            
            if (crowd != null && boss != null)
            {
                crowd.ChargeBoss(boss.transform);
            }
        }
    }
}
