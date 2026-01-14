using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ElectricityManager : MonoBehaviour
{
    public static ElectricityManager instance;

    public int totalWires = 6;
    private int connectedWires = -5;

    public TextMeshProUGUI messageText;

    public string sceneToLoad;
    public float fadeTime = 0.5f;
    public Animator fadeAnim;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        messageText.gameObject.SetActive(false);
    }

    public void WireConnected()
    {
        connectedWires++;

        if (connectedWires >= totalWires)
        {
            ElectricityRestored();
        }
    }

    void ElectricityRestored()
    {
        messageText.text = "Electricity restored";
        messageText.gameObject.SetActive(true);
        Debug.Log("test");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
            PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
            PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
            PlayerPrefs.SetInt("HasReturnPos", 1);
        }

        PlayerPrefs.SetInt("Puzzle_2_Completed", 1);

        StartCoroutine(FadeAndLoadScene());
    }



    IEnumerator FadeAndLoadScene()
    {
        fadeAnim.Play("FadeToBlack");
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }
}