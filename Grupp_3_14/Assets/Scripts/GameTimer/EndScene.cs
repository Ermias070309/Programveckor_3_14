using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{


    public void PlayGame()
    {
        // Starta timern när spelet börjar
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StartTimer();
        }

        SceneManager.LoadSceneAsync(3);
    }
}
