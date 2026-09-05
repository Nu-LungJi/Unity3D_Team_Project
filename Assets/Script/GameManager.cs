using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] GiantGolem = new GameObject[2];
    public GameObject SpawnPoint;

    void Start()
    {
        InvokeRepeating("Spawn_Golem", 10f, 14f);
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    void Spawn_Golem(){
        int i = Random.Range(0, 3);
        Instantiate(GiantGolem[i], SpawnPoint.transform.position, Quaternion.identity);
    }
}
