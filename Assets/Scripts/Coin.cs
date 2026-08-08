using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    public float rotateSpeed = 100f;

    void Update()
    {
        // Use Space.World so it spins like a coin on a table, 
        // even though the cylinder is rotated 90 degrees!
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
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
