using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject enemyObject;
    public GameObject[] respawns; 
    public AudioManager audioManager;


    // Start is called before the first frame update
    void Awake()
    {
        PlayerBehavior.audioManager = audioManager;
        PlayerBehavior.gameManagement = this;
        CreatePlayer();
        EnemyBehavior.audioManager = audioManager;
        EnemyBehavior.gameManagement = this;
        CreateEnemy();
        
    }

    public void CreateEnemy()
    {
        var enemy = Instantiate(enemyObject, respawns[1].transform.position, Quaternion.identity);
        var enemyBehavior = enemy.GetComponent<EnemyBehavior>();
    }

    public void CreatePlayer()
    {
        var player = Instantiate(playerObject, respawns[0].transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
