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
            
            // 1. Stop the player from moving forward
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            // 2. Tell the GameManager to calculate win/loss
            CrowdManager crowd = other.GetComponent<CrowdManager>();
            if (crowd != null && GameManager.instance != null)
            {
                GameManager.instance.FinishLineReached(crowd);
            }
        }
    }
}
