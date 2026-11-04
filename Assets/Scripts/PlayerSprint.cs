using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSprint : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;

    private PlayerController playerMovement;

    void Start()
    {
        // Grab the reference to your existing movement script
        playerMovement = GetComponent<PlayerController>();

        // Sync the starting speed just in case
        if (playerMovement != null)
        {
            playerMovement.moveSpeed = normalSpeed;
        }
    }

    void Update()
    {
        if (playerMovement == null || playerMovement.gameManager == null) return;

        // Don't process sprinting if the game isn't actively playing
        if (!playerMovement.gameManager.isPlaying) return;

        // Check if the Left Shift key is currently held down
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            playerMovement.moveSpeed = sprintSpeed;
        }
        else
        {
            playerMovement.moveSpeed = normalSpeed;
        }
    }
}

