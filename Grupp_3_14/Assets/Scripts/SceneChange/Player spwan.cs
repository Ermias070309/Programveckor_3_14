using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerspwan : MonoBehaviour
{
    


        [SerializeField] private Transform defaultSpawn;

        void Start()
        {
            string spawnID = PlayerPrefs.GetString("SpawnID", "Start");

            Transform spawnPoint = GameObject.Find("Spawn_" + spawnID)?.transform;

            if (spawnPoint != null)
                transform.position = spawnPoint.position;
            else
                transform.position = defaultSpawn.position;

            PlayerPrefs.DeleteKey("SpawnID");
        }
    
}
