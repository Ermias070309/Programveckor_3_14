using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public static GameManager Instance;

    public GameObject PipeHolder;
    public GameObject[] Pipes;
    public bool win = false;
    
    //Scene och fade variabler
    public string sceneToLoad;
    public float fadeTime = 0.5f;
    public Animator fadeAnim;

    [SerializeField]
    int totalPipes = 0;
    [SerializeField]
    int correcteedPipes = 0;

    // Start is called before the first frame update
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
        correcteedPipes += 1;


        Debug.Log("Correct Move");


        if(correcteedPipes == totalPipes)
        {
            Debug.Log("You win");
            win = true;
            if (win == true)
            {
            
                StartCoroutine(FadeAndLoadScene());

           
            }
        }


       

    }
    IEnumerator FadeAndLoadScene()
    {
        fadeAnim.Play("FadeToBlack");
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneToLoad);
    }



    public void wrongMove()
    {
        correcteedPipes -= 1;
    }


}
