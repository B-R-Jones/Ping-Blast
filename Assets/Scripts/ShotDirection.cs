using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotDirection : MonoBehaviour
{
    // Game entities
    private GameObject field;
    private GameObject player;
    private GameObject enemy;
    private GameObject manager;
    private ManagerScript managerScript;

    // Entity and movement attributes
    [HideInInspector] public string pORe; // Accessed by PlayerController/FireControl
    private float lifeTimer;
    [HideInInspector] public Vector2 destination; // Accessed by PlayerController/ManagerScript
    private float flightSpeed;

    // Start is called before the first frame update
    void Start()
    {
        LoadPlaySettings();
        DetermineColliders();
        Launch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (managerScript.gameOn) { Decay(); } else { Destroy(gameObject); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string collTag = collision.gameObject.tag;
        if (collTag = "Enemy" && pORe == "p") { HitEnemy(); }
        if (collTag = "Player" && pORe == "e") { HitPlayer(); }
        if (collTag = "Powerup") { HitPowerup(); }
        if (collTag = "Bullet" && pORe == "p") { IncreaseMultiplier(); }
    }

    private void LoadPlaySettings()
    {
        // Game entity settings
        field = GameObject.FindGameObjectWithTag("BulletIgnore");
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        manager = GameObject.FindGameObjectWithTag("GameController");
        managerScript = manager.GetComponent<ManagerScript>();

        // Entity settings
        lifeTimer = 5.0f;
        flightSpeed = 10.0f;
    }

    private void DetermineColliders()
    {
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
    }

    private void HitEnemy()
    {
        managerScript.enemyAlive = false;
        managerScript.score += 50 * managerScript.scoreMultiplier;
        managerScript.spawn = collision.transform;
        managerScript.scoreMultiplier = 1;
        Destroy(collision.gameObject);
        Destroy(gameObject);
    }

    private void HitPlayer()
    {
        managerScript.playerAlive = false;
        Destroy(collision.gameObject);
        Destroy(gameObject);
    }

    private void HitPowerup()
    {
            if (pORe == "p") { managerScript.scoreMultiplier += 3; }
            if (pORe == "e") { managerScript.score -= 2; }
            Destroy(collision.gameObject);
            Destroy(gameObject);
    }

    private void IncreaseMultiplier()
    {
            managerScript.scoreMultiplier += 1;
            Destroy(collision.gameObject);
            Destroy(gameObject);
    }

    private void Launch()
    {
        Vector3 myPos = new Vector3(GetComponent<Rigidbody2D>().position);
        Vector2 direction = new Vector2(destination.x - myPos.x, destination.y - myPos.y).normalized;
        GetComponent<Rigidbody2D>().velocity = direction * flightSpeed;
    }

    private void Decay()
    {
        if (timer < 0) { Destroy(gameObject); } else { lifeTimer -= Time.deltaTime; }
    }
}
