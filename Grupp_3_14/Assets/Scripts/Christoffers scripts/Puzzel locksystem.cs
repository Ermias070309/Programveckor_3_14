using UnityEngine;
using TMPro;
using System.Collections;

public class FinalPuzzleUnlock : MonoBehaviour
{
    [Header("Required puzzles")]
    [SerializeField]
    private string[] requiredPuzzles =
    {
        "Puzzle_1",
        "Puzzle_2",
        "Puzzle_3"
    };

    private Interaction interaction;

    private void Awake()
    {
        interaction = GetComponent<Interaction>();
    }

    private void Start()
    {
        CheckUnlock();
    }

    private void CheckUnlock()
    {
        foreach (string puzzle in requiredPuzzles)
        {
            if (PlayerPrefs.GetInt(puzzle + "_Completed", 0) == 0)
            {
                // Något puzzle ej klart → lås sista
                if (interaction != null)
                    interaction.enabled = false;

                return;
            }
        }

        // Alla puzzles klara → lås upp
        if (interaction != null)
            interaction.enabled = true;
    }
}