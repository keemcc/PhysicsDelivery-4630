using UnityEngine; 
  
public class GameManager : MonoBehaviour 
 { 
     public int targetScore = 3; 
     public int startingAttempts = 3; 
  
    private int currentScore = 0; 
     private int currentAttempts; 
  
    public bool isPlaying = true; 
  
    void Start() 
     { 
         currentAttempts = startingAttempts; 
     } 
  
    public void AddScore() 
     { 
         if (!isPlaying) 
             return; 
  
        currentScore++; 
  
        Debug.Log( 
             "Score: " + 
             currentScore + 
             " / " + 
             targetScore 
         ); 
  
        if (currentScore >= targetScore) 
         { 
             WinGame(); 
         } 
     } 
  
    public void LoseAttempt() 
     { 
         if (!isPlaying) 
             return; 
  
        currentAttempts--; 
  
        Debug.Log( 
             "Attempts: " + 
             currentAttempts 
         ); 
  
        if (currentAttempts <= 0) 
         { 
             LoseGame(); 
         } 
     } 
  
    void WinGame() 
     { 
         isPlaying = false; 
  
        Debug.Log("YOU WIN!"); 
     } 
  
    void LoseGame() 
     { 
         isPlaying = false; 
  
        Debug.Log("GAME OVER"); 
     } 
 } 
