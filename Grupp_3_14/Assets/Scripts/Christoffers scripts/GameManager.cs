using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject PipeHolder;
    public GameObject[] Pipes;

    [Header("Scene")]
    public string sceneToLoad;

    [Header("Spawn")]
    public string returnSpawnID = "Puzzle1Exit";

    [Header("Fade")]
    public float fadeTime = 0.5f;
    public Animator fadeAnim;

    private int totalPipes = 0;
    private int correctedPipes = 0;

    void Start()
    {
        totalPipes = PipeHolder.transform.childCount;
        Pipes = new GameObject[totalPipes];

        for (int i = 0; i < Pipes.Length; i++)
        {
            Pipes[i] = PipeHolder.transform.GetChild(i).gameObject;
        }

        Instance = this;
    }

    public void correctMove()
    {
        correctedPipes++;

        if (correctedPipes == totalPipes)
        {
            // Markera pussel klart
            PlayerPrefs.SetInt("Puzzle_1_Completed", 1);

            // Sätt spawn för återkomst
            PlayerPrefs.SetString("SpawnID", returnSpawnID);

            StartCoroutine(FadeAndLoadScene());
        }
    }

    public void wrongMove()
    {
        correctedPipes--;
    }

    IEnumerator FadeAndLoadScene()
    {
        if (fadeAnim != null)
            fadeAnim.Play("FadeToBlack");

        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }
}
