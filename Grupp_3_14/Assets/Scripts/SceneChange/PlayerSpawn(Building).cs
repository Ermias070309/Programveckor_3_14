using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    private void Start()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", "");

        if (lastScene == SceneManager.GetActiveScene().name)
        {
            float x = PlayerPrefs.GetFloat("PlayerX", transform.position.x);
            float y = PlayerPrefs.GetFloat("PlayerY", transform.position.y);

            transform.position = new Vector2(x, y);
        }
    }
}
