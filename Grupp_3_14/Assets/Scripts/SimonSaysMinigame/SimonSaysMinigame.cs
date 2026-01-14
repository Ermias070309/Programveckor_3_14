using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public float lightSpeed = 0.5f;

    void Start()
    {
        ResetGame();
    }

    public void OpenPanel()
    {
        simonSaysGamePanel.SetActive(true);
        ResetGame();
    }

    public void ClosePanel()
    {
        simonSaysGamePanel.SetActive(false);
    }

    public void ButtonClickOrder(int button)
    {
        buttonsClicked++;

        if (button == lightOrder[buttonsClicked - 1])
        {
            Debug.Log("Pass");
            passed = true;
        }
        else
        {
            Debug.Log("Failed");
            passed = false;
            won = false;
            StartCoroutine(ColorBlink(red, true)); // Fel blink
            return;
        }

        if (buttonsClicked == level)
        {
            if (level == 5 && passed)
            {
                Debug.Log("You won!");
                won = true;
                StartCoroutine(ColorBlink(green, false)); // Vinst blink
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
            // Blink alla knappar och rowLights
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

        // Efter blink, återställ alla lampor
        ResetLights();

        EnableInteractableButtons();

        if (isError)
        {
            ResetGame();
        }
        else if (won)
        {
            ClosePanel();
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

            // Återställ ljus direkt efter blink
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

        // Skapa ny slumpordning
        for (int i = 0; i < lightOrder.Length; i++)
            lightOrder[i] = Random.Range(0, buttons.Length);

        ResetLights();

        StartCoroutine(ColorOrder());
    }
}
