using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
   

    public void PlayGame()
    {
        // Starta timern när spelet börjar
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StartTimer();
        }


        PlayerPrefs.DeleteAll();

        SceneManager.LoadSceneAsync(1);
    }
}
