using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Game entities
    private GameObject field;
    private GameObject player;
    private GameObject enemy;
    private GameObject manager;
    private ManagerController managerController;

    // Entity and movement attributes
    public string pORe; // Accessed by PlayerController/FireControl
    private float lifeTimer;
    public Vector2 destination; // Accessed by PlayerController/ManagerController
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
        if (managerController.gameOn) { Decay(); } else { Destroy(gameObject); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string collTag = collision.gameObject.tag;
        if (collTag == "Enemy" && pORe == "p") { HitEnemy(collision); }
        if (collTag == "Player" && pORe == "e") { HitPlayer(collision); }
        if (collTag == "Powerup") { HitPowerup(collision); }
        if (collTag == "Bullet" && pORe == "p") { IncreaseMultiplier(collision); }
    }

    private void LoadPlaySettings()
    {
        // Game entity settings
        field = GameObject.FindGameObjectWithTag("BulletIgnore");
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        manager = GameObject.FindGameObjectWithTag("GameController");
        managerController = manager.GetComponent<ManagerController>();

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

    private void HitEnemy(Collision2D collision)
    {
        managerController.enemyAlive = false;
        managerController.moveCounter = collision.gameObject.GetComponent<EnemyController>().stepCounter;
        managerController.score += 50 * managerController.scoreMultiplier;
        managerController.scoreMultiplier = 1;
        Destroy(collision.gameObject);
        Destroy(gameObject);
    }

    private void HitPlayer(Collision2D collision)
    {
        managerController.playerAlive = false;
        managerController.moveIndex = collision.gameObject.GetComponent<PlayerController>().stepIndex;
        Destroy(collision.gameObject);
        Destroy(gameObject);
    }

    private void HitPowerup(Collision2D collision)
    {
        if (pORe == "p") { managerController.scoreMultiplier += 3; }
        if (pORe == "e") { managerController.score -= 2; }
        Destroy(collision.gameObject);
        Destroy(gameObject);
    }

    private void IncreaseMultiplier(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<BulletController>())
        {
            if (collision.gameObject.GetComponent<BulletController>().pORe == "e")
            {
                managerController.scoreMultiplier += 1;
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }

    }

    private void Launch()
    {
        Vector2 myPos = new(GetComponent<Rigidbody2D>().position.x,
                            GetComponent<Rigidbody2D>().position.y);
        Vector2 direction = new Vector2(destination.x - myPos.x, 
                                        destination.y - myPos.y).normalized;
        GetComponent<Rigidbody2D>().velocity = direction * flightSpeed;
    }

    private void Decay()
    {
        if (lifeTimer < 0) { Destroy(gameObject); } else { lifeTimer -= Time.deltaTime; }
    }
}
