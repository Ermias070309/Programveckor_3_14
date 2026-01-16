using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class SimonSaysMinigame : MonoBehaviour
{
    [SerializeField] GameObject[] buttons;
    [SerializeField] GameObject[] lightArray;
    [SerializeField] GameObject[] rowLights;
    [SerializeField] GameObject simonSaysGamePanel;
    [SerializeField] int[] lightOrder;

    int level = 1;
    int buttonsClicked;
    int colorOrderRunCount = -1;

    bool passed = false;
    bool won = false;

    Color32 red = new Color32(255, 39, 0, 255);
    Color32 green = new Color32(0, 255, 0, 255);
    Color32 invisible = new Color32(255, 255, 255, 0);
    Color32 white = new Color32(255, 255, 255, 255);

    [Header("Gameplay")]
    public float lightSpeed = 0.5f;

    [Header("Scene")]
    public string sceneToLoad;

    [Header("Spawn")]
    public string returnSpawnID = "PuzzleSimonExit";

    [Header("Fade")]
    public float fadeTime = 0.5f;
    public Animator fadeAnim;

    [Header("Puzzle Settings")]
    public string puzzleID = "Puzzle_Simon";

    void Start()
    {
        // Om pusslet redan är klart – visa inte panelen
        if (PlayerPrefs.GetInt(puzzleID + "_Completed", 0) == 1)
        {
            simonSaysGamePanel.SetActive(false);
            return;
        }

        simonSaysGamePanel.SetActive(true);
        StartCoroutine(StartGameNextFrame());
    }

    IEnumerator StartGameNextFrame()
    {
        yield return null;
        ResetGame();
    }

    public void ButtonClickOrder(int button)
    {
        buttonsClicked++;

        if (button == lightOrder[buttonsClicked - 1])
        {
            passed = true;
        }
        else
        {
            passed = false;
            won = false;
            StartCoroutine(ColorBlink(red, true));
            return;
        }

        if (buttonsClicked == level)
        {
            if (level == 5 && passed)
            {
                won = true;
                StartCoroutine(ColorBlink(green, false));
            }
            else if (passed)
            {
                level++;
                passed = false;
                StartCoroutine(ColorOrder());
            }
        }
    }

    IEnumerator ColorBlink(Color32 colorToBlink, bool isError)
    {
        DisableInteractableButtons();

        for (int j = 0; j < 3; j++)
        {
            foreach (var btn in buttons)
                btn.GetComponent<Image>().color = colorToBlink;

            foreach (var row in rowLights)
                row.GetComponent<Image>().color = colorToBlink;

            yield return new WaitForSeconds(0.5f);

            foreach (var btn in buttons)
                btn.GetComponent<Image>().color = white;

            foreach (var row in rowLights)
                row.GetComponent<Image>().color = white;

            yield return new WaitForSeconds(0.5f);
        }

        ResetLights();
        EnableInteractableButtons();

        if (isError)
        {
            ResetGame();
        }
        else if (won)
        {
            // Markera pussel klart
            PlayerPrefs.SetInt(puzzleID + "_Completed", 1);

            // Sätt spawnpunkt för återkomst
            PlayerPrefs.SetString("SpawnID", returnSpawnID);

            PlayerPrefs.Save();

            // Fade + scenbyte
            StartCoroutine(LoadSceneAfterFade());
        }
    }

    IEnumerator ColorOrder()
    {
        buttonsClicked = 0;
        colorOrderRunCount++;
        DisableInteractableButtons();

        for (int i = 0; i <= colorOrderRunCount; i++)
        {
            lightArray[lightOrder[i]].GetComponent<Image>().color = invisible;
            yield return new WaitForSeconds(lightSpeed);

            lightArray[lightOrder[i]].GetComponent<Image>().color = green;
            rowLights[i].GetComponent<Image>().color = green;
            yield return new WaitForSeconds(lightSpeed);

            lightArray[lightOrder[i]].GetComponent<Image>().color = invisible;
        }

        EnableInteractableButtons();
    }

    void DisableInteractableButtons()
    {
        foreach (var btn in buttons)
            btn.GetComponent<Button>().interactable = false;
    }

    void EnableInteractableButtons()
    {
        foreach (var btn in buttons)
            btn.GetComponent<Button>().interactable = true;
    }

    void ResetLights()
    {
        foreach (var light in lightArray)
            light.GetComponent<Image>().color = invisible;

        foreach (var row in rowLights)
            row.GetComponent<Image>().color = white;
    }

    void ResetGame()
    {
        StopAllCoroutines();

        level = 1;
        buttonsClicked = 0;
        colorOrderRunCount = -1;
        won = false;
        passed = false;

        for (int i = 0; i < lightOrder.Length; i++)
            lightOrder[i] = Random.Range(0, buttons.Length);

        ResetLights();
        EnableInteractableButtons();
        StartCoroutine(ColorOrder());
    }

    IEnumerator LoadSceneAfterFade()
    {
        if (fadeAnim != null)
            fadeAnim.Play("FadeToBlack");

        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }
}

