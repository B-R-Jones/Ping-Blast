using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotDirection : MonoBehaviour
{
    public Vector2 destination;
    private GameObject field;
    private GameObject player;
    private GameObject enemy;
    private GameObject manager;
    private float flightSpeed;
    private float timer;
    public string pORe;
    // Start is called before the first frame update
    void Start()
    {
        timer = 5.0f;
        flightSpeed = 10.0f;
        field = GameObject.FindGameObjectWithTag("BulletIgnore");
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        manager = GameObject.FindGameObjectWithTag("GameController");

        foreach (Collider2D col in field.GetComponents<Collider2D>())
        {
            Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
        }

        if (pORe == "p")
        {
            foreach (Collider2D col in player.GetComponents<Collider2D>())
            {
                Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
            }
        }

        if (pORe == "e")
        {
            foreach (Collider2D col in enemy.GetComponents<Collider2D>())
            {
                Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
            }
        }

        Launch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0) { Destroy(gameObject); }
    }

    void Launch()
    {
        Debug.Log($"BUL V[{destination}]");
        Vector2 direction = new Vector2(destination.x - GetComponent<Rigidbody2D>().position.x, destination.y - GetComponent<Rigidbody2D>().position.y).normalized;
        GetComponent<Rigidbody2D>().velocity = direction * flightSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit!");
        Debug.Log($"PORE: {pORe}");

        if (collision.gameObject.tag == "Enemy")
        {
            manager.GetComponent<ManagerScript>().enemyAlive = false;
            manager.GetComponent<ManagerScript>().spawn = collision.transform;
        }

        if (collision.gameObject.tag == "Player")
        {
            manager.GetComponent<ManagerScript>().playerAlive = false;
        }

        Destroy(collision.gameObject);
        Destroy(gameObject);
    }
}
