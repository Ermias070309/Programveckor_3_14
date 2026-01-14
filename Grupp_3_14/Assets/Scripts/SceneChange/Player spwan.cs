using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerspwan : MonoBehaviour
{
    [SerializeField] private Vector3 startSpawnPoint;

    void Start()
    {
        if (PlayerPrefs.GetInt("HasReturnPos", 0) == 1)
        {
            transform.position = new Vector3(
                PlayerPrefs.GetFloat("PlayerX"),
                PlayerPrefs.GetFloat("PlayerY"),
                PlayerPrefs.GetFloat("PlayerZ")
            );
            PlayerPrefs.SetInt("HasReturnPos", 0);
        }
        else
        {


            PlayerPrefs.DeleteKey("Puzzle_1_Completed");
            PlayerPrefs.DeleteKey("Puzzle_2_Completed");
            transform.position = startSpawnPoint;
        }
    }
}
