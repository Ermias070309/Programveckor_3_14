using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad;

    [Header("Spawn")]
    [SerializeField] private string exitSpawnID;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private SpriteRenderer interactIcon;

    [Header("Fade (optional)")]
    [SerializeField] private Animator fadeAnim;
    [SerializeField] private float fadeTime = 0.3f;

    private bool playerInRange;

    private void Awake()
    {
        // Säkerställ att ikonen alltid är dold från start
        if (interactIcon != null)
            interactIcon.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;

        if (interactIcon != null)
            interactIcon.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;

        if (interactIcon != null)
            interactIcon.enabled = false;
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (interactIcon != null)
                interactIcon.enabled = false;

            // Säg till nästa scen var spelaren ska spawnas
            PlayerPrefs.SetString("SpawnID", exitSpawnID);

            if (fadeAnim != null)
            {
                fadeAnim.Play("FadeToBlack");
                Invoke(nameof(LoadScene), fadeTime);
            }
            else
            {
                LoadScene();
            }
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
