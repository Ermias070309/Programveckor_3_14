using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Mainmenu : MonoBehaviour
{
    
    public void PlayGame()
    {
        PlayerPrefs.DeleteKey("Puzzle_1_Completed");
        PlayerPrefs.DeleteKey("Puzzle_2_Completed");

        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY"); 
        PlayerPrefs.DeleteKey("PlayerZ");

        SceneManager.LoadSceneAsync(1);
       
    }


}
