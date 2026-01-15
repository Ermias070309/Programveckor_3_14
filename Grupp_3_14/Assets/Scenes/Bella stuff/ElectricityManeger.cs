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

    [Header("Scene")]
    public string sceneToLoad;

    [Header("Spawn")]
    public string returnSpawnID = "Puzzle2Exit";

    [Header("Fade")]
    public float fadeTime = 0.5f;
    public Animator fadeAnim;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (messageText != null)
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
        if (messageText != null)
        {
            messageText.text = "Electricity restored";
            messageText.gameObject.SetActive(true);
        }

        //  Sätt spawn-punkt för när vi kommer tillbaka
        PlayerPrefs.SetString("SpawnID", returnSpawnID);

        //  Markera pussel klart
        PlayerPrefs.SetInt("Puzzle_2_Completed", 1);

        StartCoroutine(FadeAndLoadScene());
    }

    IEnumerator FadeAndLoadScene()
    {
        if (fadeAnim != null)
            fadeAnim.Play("FadeToBlack");

        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }
}