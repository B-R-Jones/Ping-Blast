using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public Rigidbody2D ghost;
    public GameObject shot;

    [HideInInspector]
    public Transform spawn;

    [HideInInspector]
    public int stepCount;

    [HideInInspector]
    public Dictionary<int, Vector2> moveList;
    [HideInInspector]
    public int moveCounter;
    [HideInInspector]
    public int moveIndex;

    public bool enemyAlive;
    public bool playerAlive;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyAlive = true;
        moveList = new Dictionary<int, Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!enemyAlive)
        {
            //add to score
            GameObject newEnemy;
            newEnemy = Instantiate(enemy, spawn);
            player.GetComponent<PlayerController>().enemy = newEnemy.GetComponent<Rigidbody2D>();
            enemyAlive = true;
        }

        if (!playerAlive)
        {
            //end game
        }
    }
}
