using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;            // starting position relative to the target; sets the initial orbit angle and distance
    public float speed = 1f;

    public float mouseSensitivity = 0.2f;  // degrees of rotation per pixel of mouse movement
    public float minPitch = 5f;
    public float maxPitch = 80f;

    public float zoomStep = 1f;       // distance moved per scroll wheel notch
    public float minDistance = 3f;
    public float maxDistance = 20f;

    private float yaw;
    private float pitch;
    private float distance;
    private Vector3 focus;

    private void Start()
    {
        distance = offset.magnitude;
        if (distance > 0f)
        {
            yaw = Mathf.Atan2(-offset.x, -offset.z) * Mathf.Rad2Deg;
            pitch = Mathf.Asin(offset.y / distance) * Mathf.Rad2Deg;
        }
        if (target != null) focus = target.position;

        LockCursor(true);
    }

    private void OnDisable()
    {
        LockCursor(false);
    }

    private void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            bool playing = GameManager.instance == null || GameManager.instance.isPlaying;

            // Free the cursor with Escape or when the game ends; click back in to re-lock it.
            if (!playing || (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame))
                LockCursor(false);
            else if (mouse.leftButton.wasPressedThisFrame)
                LockCursor(true);

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Vector2 delta = mouse.delta.ReadValue();
                yaw += delta.x * mouseSensitivity;
                pitch = Mathf.Clamp(pitch - delta.y * mouseSensitivity, minPitch, maxPitch);

                // Scroll values differ per platform (1 vs 120 per notch), so only use the direction.
                float scroll = mouse.scroll.ReadValue().y;
                if (scroll != 0f)
                    distance = Mathf.Clamp(distance - Mathf.Sign(scroll) * zoomStep, minDistance, maxDistance);
            }
        }

        // Smooth the followed point, but apply rotation immediately so the mouse feels responsive.
        focus = Vector3.Lerp(focus, target.position, speed * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = focus + rotation * new Vector3(0f, 0f, -distance);
        transform.LookAt(focus);
    }
}
