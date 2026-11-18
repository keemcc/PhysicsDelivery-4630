using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("The upward force applied to the object.")]
    [SerializeField] private float jumpForce = 15f;

    [Tooltip("Reset vertical velocity before applying force to keep bounce heights consistent.")]
    [SerializeField] private bool resetVelocityY = true;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has a Rigidbody attached
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            if (resetVelocityY)
            {
                // Clear existing falling/rising velocity to ensure consistent launch height
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            }

            // Apply an instant upward impulse force
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
