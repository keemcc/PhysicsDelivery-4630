using UnityEngine; 
  
public class GoalZone : MonoBehaviour 
 {   
    void OnTriggerEnter(Collider other) 
     { 
         if (other.CompareTag("DeliveryObject")) 
         { 
             Debug.Log("Successful Delivery!"); 
  
            GameManager.instance.AddScore(); 
  
            DeliveryObject delivery = 
                 other.GetComponent<DeliveryObject>(); 
  
            if (delivery != null) 
             { 
                 delivery.ResetDelivery(); 
             } 
         } 
     } 
 } 
