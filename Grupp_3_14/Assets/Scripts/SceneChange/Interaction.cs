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
    private Transform player;
    
    public int förstaSpawn = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!collision.CompareTag("Player"))
                return;

            playerInRange = true;

            player = collision.transform;


        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = false;
        player = null;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {

            PlayerPrefs.SetFloat("PlayerX", player. position.x);
            PlayerPrefs.SetFloat("PlayerY", player.position.y);
            PlayerPrefs.SetFloat("PlayerZ", player.position.z);

            PlayerPrefs.SetInt("HasReturnPos", 1);




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
