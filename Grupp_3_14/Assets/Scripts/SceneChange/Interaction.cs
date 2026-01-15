using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour
{
    public string sceneToLoad;
    public Animator fadeAnim;
    public float fadeTime = 0.5f;

    private bool playerInRange;
    private Transform player;

    public string puzzleID = "Puzzle_1";

    public SpriteRenderer interactIcon;

    private void Start()
    {
        // Ikonen ska vara dold från start
        if (interactIcon != null)
            interactIcon.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        // Visa inget om pusslet redan är klart
        if (PlayerPrefs.GetInt(puzzleID + "_Completed", 0) == 1)
            return;

        playerInRange = true;
        player = collision.transform;

        if (interactIcon != null)
            interactIcon.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = false;
        player = null;

        if (interactIcon != null)
            interactIcon.enabled = false;
    }

    void Update()
    {
        if (!playerInRange) return;

        if (PlayerPrefs.GetInt(puzzleID + "_Completed", 0) == 1)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerPrefs.SetFloat("PlayerX", player.position.x);
            PlayerPrefs.SetFloat("PlayerY", player.position.y);
            PlayerPrefs.SetFloat("PlayerZ", player.position.z);
            PlayerPrefs.SetInt("HasReturnPos", 1);

            if (interactIcon != null)
                interactIcon.enabled = false;

            fadeAnim.Play("FadeToBlack");
            StartCoroutine(DelayFade());
        }
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }
}