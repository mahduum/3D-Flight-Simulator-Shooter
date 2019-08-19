using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryTarget : MonoBehaviour
{
    private GameObject enemy;
    private GameObject player;

    Rigidbody rb;

    Vector3 enemyPos;
    Vector3 playerPos;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();

        enemyPos = enemy.transform.position;
        playerPos = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 marker = player.transform.position - (player.transform.position - enemy.transform.position) / 2;

        ////Vector3 markerPlacement = playerPos - (playerPos - enemyPos) / 2;

        //transform.position = marker + (enemy.transform.forward - player.transform.forward);

        //rb.AddForce(player.transform.forward);
       


    }
}
