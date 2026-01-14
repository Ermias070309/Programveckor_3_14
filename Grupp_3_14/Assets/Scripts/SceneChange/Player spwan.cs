using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerspwan : MonoBehaviour
{
    [SerializeField] private Vector3 startSpawnPoint;

    void Awake()
    {
        if (PlayerPrefs.GetInt("HasReturnPos", 0) == 1)
        {
            transform.position = new Vector3(
                PlayerPrefs.GetFloat("PlayerX"),
                PlayerPrefs.GetFloat("PlayerY"),
                PlayerPrefs.GetFloat("PlayerZ")
            );

            // Nollställ så nästa start blir normal
            PlayerPrefs.SetInt("HasReturnPos", 0);
        }
        else
        {
            transform.position = startSpawnPoint;
        }

    }
}
