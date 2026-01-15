using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buildingspawn : MonoBehaviour
{
    public string sceneToLoad;
    public Animator fadeAnim;
    public float fadeTime = 0.5f;

    private bool playerInRange;
    private Transform player;

    public string puzzleID = "Puzzle_1";

    public SpriteRenderer interactIcon;
    public string returnID = "Door_1";

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

        
        if (Input.GetKeyDown(KeyCode.E))
        {
           
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
