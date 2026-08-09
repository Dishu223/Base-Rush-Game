using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float swipeSpeed = 10f;
    public float boundaryX = 4.5f; // Limits how far left/right the player can go

    private Vector3 lastMousePosition;
    private bool isMoving = false;

    void Start()
    {
        Debug.Log("PlayerController started! Waiting for input...");
    }

    void Update()
    {
        // Don't move if the game is over!
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;
        
        // Stop forward movement when fighting the boss!
        if (GameManager.instance != null && GameManager.instance.isBossPhase) return;

        // For hypercasual, wait for the first tap to start running
        if (!isMoving && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse click detected! Starting movement.");
            isMoving = true;
        }

        if (isMoving)
        {
            MoveForward();
            HandleHorizontalMovement();
        }
    }

    private void MoveForward()
    {
        float speedMod = 1f;
        if (GameManager.instance != null) speedMod = GameManager.instance.GetLevelSpeedModifier();
        
        // Move the player forward automatically
        transform.Translate(Vector3.forward * forwardSpeed * speedMod * Time.deltaTime);
    }

    private void HandleHorizontalMovement()
    {
        // Handle horizontal swiping (works for mouse and touch in Unity)
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            // Calculate difference in mouse/finger position
            Vector3 difference = Input.mousePosition - lastMousePosition;
            
            // Move horizontally based on the drag distance
            float horizontalMove = difference.x * swipeSpeed * Time.deltaTime * 0.01f;
            
            // Apply horizontal movement
            Vector3 newPosition = transform.position + new Vector3(horizontalMove, 0, 0);
            
            // Clamp the position so player doesn't fall off the platform
            newPosition.x = Mathf.Clamp(newPosition.x, -boundaryX, boundaryX);
            
            transform.position = newPosition;
            
            // Update last mouse position for the next frame
            lastMousePosition = Input.mousePosition;
        }
    }
}
