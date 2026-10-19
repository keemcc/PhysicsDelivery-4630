using UnityEngine; 
using UnityEngine.InputSystem; 

public class PlayerController : MonoBehaviour 
{ 
    public float moveSpeed = 5f; 
    public float acceleration = 20f;  // how quickly input changes velocity; lower = slidier
    public float friction = 4f;       // how quickly the player slows with no input; lower = slides further

    private Rigidbody rb; 
    private Vector2 moveInput; 

    void Start() 
    { 
        rb = GetComponent<Rigidbody>(); 
    } 

    public Transform resetPoint;      // where this player is sent when a hazard resets them

    // Teleport the player to its reset point and stop all motion.
    public void ResetToPoint()
    {
        if (resetPoint == null) return;
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = resetPoint.position;
        transform.position = resetPoint.position;
        moveInput = Vector2.zero;
    }

void Update() 
 { 
     if (!GameManager.instance.isPlaying) 
     { 
         moveInput = Vector2.zero; 
         return; 
     } 
  
    float horizontal = 0f; 
     float vertical = 0f; 
  
    if (Keyboard.current.aKey.isPressed) 
         horizontal = -1f; 
  
    if (Keyboard.current.dKey.isPressed) 
         horizontal = 1f; 
  
    if (Keyboard.current.sKey.isPressed) 
         vertical = -1f; 
  
    if (Keyboard.current.wKey.isPressed) 
         vertical = 1f; 
  
    moveInput = 
         new Vector2(horizontal, vertical);


    }

    void FixedUpdate()
 {
     if (!GameManager.instance.isPlaying)
         return;

    Vector3 movement =
         new Vector3(
             moveInput.x,
             0f,
             moveInput.y
         );

    movement = movement.normalized;

    Vector3 v = rb.linearVelocity;
     Vector3 flat = new Vector3(v.x, 0f, v.z);

    if (movement.sqrMagnitude > 0f)
         flat = Vector3.MoveTowards(
             flat, movement * moveSpeed,
             acceleration * Time.fixedDeltaTime);
     else
         flat = Vector3.MoveTowards(
             flat, Vector3.zero,
             friction * Time.fixedDeltaTime);

    rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);
 }
} 
