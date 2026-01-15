using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Mainmenu : MonoBehaviour
{
    
    public void PlayGame()
    {
        PlayerPrefs.DeleteAll();

        SceneManager.LoadSceneAsync(1);
       
    }


}
