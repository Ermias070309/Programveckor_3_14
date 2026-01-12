using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour
{
    public string sceneToLoad;
    public Animator fadeAnim;
    public float fadeTime = 0.5f;
    private bool playerInRange;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            fadeAnim.Play("FadeToBlack");
            StartCoroutine(DelayFade());
            Debug.Log("AAAAaa");
        }
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }
}
