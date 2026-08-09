using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    public float rotateSpeed = 100f;

    public float magnetRadius = 5f;
    public float magnetSpeed = 20f;
    private Transform targetPlayer;

    void Update()
    {
        // Use Space.World so it spins like a coin on a table
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);

        if (targetPlayer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, magnetSpeed * Time.deltaTime);
        }
        else
        {
            // Find player if close enough to attract!
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && Vector3.Distance(transform.position, player.transform.position) < magnetRadius)
            {
                targetPlayer = player.transform;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Either the player leader or the follower units can pick up coins
        if (other.CompareTag("Player") || other.CompareTag("Unit"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddCoins(coinValue);
            }

            // Spawn floating text for the coin pickup (we made it smaller by passing 0.3f scale!)
            if (VFXManager.instance != null)
            {
                VFXManager.instance.SpawnFloatingText(transform.position, "+1", Color.yellow, 0.3f);
            }

            // Optional: Play a sound effect here!

            Destroy(gameObject);
        }
    }
}
