using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void StartNewGame()
    {
        
        SceneManager.LoadScene("MainStory");
    }
    public void ContinueGame()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadScene(1);
    }
    public void Exit()
    {
        Application.Quit();
    }
    
   
    
    void StartTransition()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadScene(1);
        else if (SceneManager.GetActiveScene().buildIndex == 1)
            SceneManager.LoadScene(1);
    }

}
