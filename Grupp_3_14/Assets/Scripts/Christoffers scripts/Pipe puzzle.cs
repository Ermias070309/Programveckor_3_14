using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipepuzzle : MonoBehaviour
{


    float[] rotations = { 0, 90, 180, 270 };

    public float[] correctRotation;

    [SerializeField]
    bool isPlaced = false;

    int PossibleRots = 1;


    GameManager gameManager;


    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    void Start()
    {
        PossibleRots = correctRotation.Length;

        int rand = Random.Range(0, rotations.Length);
        transform.eulerAngles = new Vector3(0, 0, rotations[rand]);

        CheckPlacement();
    }

    private void OnMouseDown()
    {
        transform.Rotate(0, 0, 90);
        CheckPlacement();
    }

    void CheckPlacement()
    {
        float z = Mathf.Round(transform.eulerAngles.z);

        bool isCorrect = false;

        for (int i = 0; i < correctRotation.Length; i++)
        {
            if (z == correctRotation[i])
            {
                isCorrect = true;
                break;
            }
        }

        //  Endast om status ändras
        if (isCorrect && !isPlaced)
        {
            isPlaced = true;
            gameManager.correctMove();
        }
        else if (!isCorrect && isPlaced)
        {
            isPlaced = false;
            gameManager.wrongMove();
        }
    }

}
