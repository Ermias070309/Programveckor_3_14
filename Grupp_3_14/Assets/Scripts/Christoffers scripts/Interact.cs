using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interact : MonoBehaviour
{
    public class SceneChanger2D : MonoBehaviour
    {
        public string sceneToLoad;
        private bool playerIsNear = false;

        void Update()
        {
            if (playerIsNear && Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene("Christoffer Scene");
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerIsNear = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerIsNear = false;
            }
        }
    }
}
