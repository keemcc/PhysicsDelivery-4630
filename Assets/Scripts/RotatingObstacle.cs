using UnityEngine; 
using UnityEngine.InputSystem; 
  
public class RotatingObstacle : MonoBehaviour 
 { 
     public float rotationSpeed = 90f; 
     public bool isActive = true; 
  
    void Start() 
     { 
         Debug.Log("RotatingObstacle is running"); 
     } 
  
    void Update() 
     { 
        if (isActive) 
         { 
             float rotationThisFrame = 
                 rotationSpeed * Time.deltaTime; 
  
            transform.Rotate( 
                 0f, 
                 0f, 
                 rotationThisFrame
             ); 
         } 
     } 
 } 
