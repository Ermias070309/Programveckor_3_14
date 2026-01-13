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

    int level = 0;
    int buttonsClicked;
    int colorOrderRunCount = 0;

    bool passed = false;
    bool won = false;

    Color32 red = new Color32(255, 39, 0, 255);
    Color32 green = new Color32(0, 255, 0, 255);
    Color32 invisible = new Color32(255, 255, 255, 0);
    Color32 white = new Color32(255, 255, 255, 255);

    public float lightSpeed;

    public void ButtonClickOrder(int button)
    {
        buttonsClicked++;

        if(button == lightOrder[buttonsClicked - 1])
        {
            Debug.Log("Pass");
            passed = true;
        }
        else
        {
            Debug.Log("Failed");
            won = false;
            passed = false;
            StartCoroutine(ColorBlink(red));
        }
        if (buttonsClicked ==level && passed == true && buttonsClicked != 5)
        {
            level++;
            passed = false;
            StartCoroutine(ColorOrder());
        }
        if (buttonsClicked == level && passed == true && buttonsClicked == 5)
        {
            Debug.Log("You won");
            won = true;
            StartCoroutine(ColorBlink(green));
        }
    }

    public void ClosePanel()
    {
        simonSaysGamePanel.SetActive(false);
    }

    public void OpenPanel()
    {
        simonSaysGamePanel.SetActive(true);
    }

    IEnumerator ColorBlink(Color32 colorToBlink)
    {
        DisableInteractableButtons();

        for(int j = 0; j < 3; j++)
        {
            Debug.Log("I run this many times" + j);

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<Image>().color = colorToBlink;
            }
            for (int i = 5; i < rowLights.Length; i++)
            {
                rowLights[i].GetComponent<Image>().color = colorToBlink;
            }

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<Image>().color = white;
            }
            for (int i = 5; i < rowLights.Length; i++)
            {
                rowLights[i].GetComponent<Image>().color = white;
            }

            yield return new WaitForSeconds(0.5f);
        }
        if (won == true)
        {
            Debug.Log("put won stuff here");
            ClosePanel();
        }
        EnableInteractableButtons();
        ResetGame();
    }

    IEnumerator ColorOrder()
    {
        buttonsClicked = 0;
        colorOrderRunCount++;
        DisableInteractableButtons();
        for (int i = 0; i <= colorOrderRunCount; i++)
        {
            if (level >= colorOrderRunCount)
            {
                lightArray[lightOrder[i]].GetComponent<Image>().color = invisible;
                yield return new WaitForSeconds(lightSpeed);

                lightArray[lightOrder[i]].GetComponent<Image>().color = green;
                yield return new WaitForSeconds(lightSpeed);

                rowLights[i].GetComponent<Image>().color = green;
            }
        }
        EnableInteractableButtons();
    }

    void DisableInteractableButtons()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Button>().interactable = false;
        }
    }

    void EnableInteractableButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Button>().interactable = true;
        }
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

        for (int i = 0; i < rowLights.Length; i++)
            rowLights[i].GetComponent<Image>().color = white;

        StartCoroutine(ColorOrder());
    }

}
