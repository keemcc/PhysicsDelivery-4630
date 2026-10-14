using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Ensure you are using the New Input System

public class ExpandPlayer : MonoBehaviour
{
    private bool isScaling = false;
    private Rigidbody rb;

    // Adjust these values in the Unity Inspector
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);
    public float duration = 0.2f;

    //Base force values
    public float pushRadius = 5f;       // How far the push force reaches
    public float explosionForce = 15f;  // How strong the push is
    public float upwardModifier = 1f;   // Adds a slight lift to the pushed objects


    private Vector3 originalScale;

    void Start()
    {
        // Save the starting size of the object
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if Space is pressed and we aren't already animation scaling
        if (Keyboard.current.spaceKey.isPressed && !isScaling)
        {
            StartCoroutine(ScaleOverTime());
        }
    }

    IEnumerator ScaleOverTime()
    {
        isScaling = true;
        //apply force
        Collider[] colliders = Physics.OverlapSphere(transform.position, pushRadius);

        foreach (Collider hit in colliders)
        {
            // Don't apply the force to yourself!
            if (hit.gameObject == this.gameObject) continue;

            Rigidbody targetRb = hit.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                // 2. Apply explosion force pushing outwards from your position
                targetRb.AddExplosionForce(
                    explosionForce,
                    transform.position,
                    pushRadius,
                    upwardModifier,
                    ForceMode.Impulse
                );
            }
        }

        // 1. Scale Up
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, currentTime / duration);
            yield return null;
        }

        // 2. Scale Back Down
        currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, currentTime / duration);
            yield return null;
        }

        // Ensure it returns exactly to the original size
        transform.localScale = originalScale;
        isScaling = false;
    }
}

