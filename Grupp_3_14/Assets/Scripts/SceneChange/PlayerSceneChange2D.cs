using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader2D : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    private bool playerInTrigger = false;
    private Transform player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            player = null;
        }
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // Spara position
            PlayerPrefs.SetFloat("PlayerX", player.position.x);
            PlayerPrefs.SetFloat("PlayerY", player.position.y);
            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
