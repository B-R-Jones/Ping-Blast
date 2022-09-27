using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player components
    private Rigidbody2D myRigidbody;
    private Vector2 myPos;
    private Vector2 newPos;

    // Movement modifiers and firing 
    private float movementMultiplier;
    private float shiftSpeed;

    // Other game entities
    private GameObject manager;
    private ManagerScript managerScript;
    [HideInInspector] public Rigidbody2D enemy; // Accessed by ManagerScript
    [HideInInspector] public Rigidbody2D ghost; // Accessed by ManagerScript

    // Vector positions of the ghost
    private Vector2 ghostPos;
    private Vector2 newGhostPos;
    //private Vector2 enemyPos;

    // Enemy movement dictionary, steppers, etc.
    private Dictionary<int, Vector2> enemySteps;
    //private int stepCounter;
    private int stepIndex;
    //private float enemyMoveTimer;
    //private bool enemyDetected;

    // Weapon attributes and modifiers
    public GameObject shot; // Leave open for easy swapping of shots in dev mode
    [HideInInspector] public int shotNumber; // Accessed by SPManager
    [HideInInspector] public float shotTimer; // Accessed by RFManager
    [HideInInspector] public bool rapidFireOn; // Accessed by RFManager
    [HideInInspector] public bool spreadShotOn; // Accessed by SPManager
    private float rapidFireTimer;
    private float spreadShotTimer;

    // Start is called before the first frame update
    void Start()
    {
        LoadPlaySettings();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (managerScript.gameOn)
        {
            DetectEnemy();
            SetPositions();
            MoveSquare();
            DetectFire();
            SetGhost();

            //enemyMoveTimer -= Time.deltaTime;
            shotTimer -= Time.deltaTime;
        }
    }

    private void LoadPlaySettings()
    {
        // Manager settings
        manager = GameObject.FindGameObjectWithTag("GameController");
        managerScript = manager.GetComponent<ManagerScript>();

        // Player settings
        myRigidbody = GetComponent<Rigidbody2D>;
        myPos = myRigidBody.position;

        // Ghost settings
        ghost = GameObject.FindGameObjectWithTag("Ghost").gameObject.GetComponent<Rigidbody2D>();

        //Enemy and enemy movement
        enemy = GameObject.FindGameObjectWithTag("Enemy").gameObject.GetComponent<Rigidbody2D>();
        //enemyDetected = true;
        enemySteps = new Dictionary<int, Vector2>();
        enemySteps = managerScript.moveList;
        //stepCounter = managerScript.moveCounter;
        stepIndex = managerScript.moveIndex;
        //enemyMoveTimer = Random.Range(0f, 3f);

        // Powerup and firing settings
        rapidFireOn = false;
        rapidFireTimer = 10.0f;
        spreadShotOn = false;
        spreadShotTimer = 10.0f;
        shotTimer = 1.0f;
        shotNumber = 1;

        // Movement settings
        shiftSpeed = 0.0f;
    }

    //private void DetectEnemy()
    //{
    //    //if (GameObject.FindGameObjectWithTag("Enemy") != null)
    //    //{
    //    //    enemy = GameObject.FindGameObjectWithTag("Enemy").gameObject.GetComponent<Rigidbody2D>();
    //    //    //if (enemy != null) { enemyDetected = true; }
    //    //}
    //    //else
    //    //{
    //    //    enemyDetected = false;
    //    //}
    //}

    private void SetPositions()
    {
        //if (enemyDetected) { enemyPos = enemy.position; }
        newPos = GetMovement(myPos);
    }

    private Vector2 GetMovement(Vector2 pV2)
    {
        if (Input.GetKey(KeyCode.W)) { pV2.y += 1f; }
        if (Input.GetKey(KeyCode.S)) { pV2.y -= 1f; }
        if (Input.GetKey(KeyCode.D)) { pV2.x += 1f; }
        if (Input.GetKey(KeyCode.A)) { pV2.x -= 1f; }
        if (Input.GetKey(KeyCode.LeftShift)) { shiftSpeed = 1.25f; } else { shiftSpeed = 1; }

        return pV2;
    }

    private void MoveSquare()
    {
        Vector2 direction = new Vector2(myPos.x - newPos.x, myPos.y - newPos.y).normalized;
        GetComponent<Rigidbody2D>().velocity = -1 * movementMultiplier * shiftSpeed * direction;
    }

    //// This can be moved to the enemy controller (FireControl) in the future
    //private void MoveSquareEnemy()
    //{
    //    if (enemySteps.ContainsKey(stepCounter) && enemySteps.Count >= 1)
    //    {
    //        if (CheckForPowerup() != null)
    //        {
    //            if (CheckForPowerup().position.y >= 0.5f)
    //            {
    //                enemy.position = Vector2.MoveTowards(enemyPos, CheckForPowerup().position, movementMultiplier * Time.deltaTime);
    //            }
    //        }
    //        else
    //        {
    //            enemy.position = Vector2.MoveTowards(enemyPos, enemySteps[stepCounter], movementMultiplier * Time.deltaTime);
    //        }
    //    }
    //    stepCounter++;
    //    enemySteps.Remove(stepCounter - 1);
    //}

    //// This can be moved to the enemy controller (FireControl) in the future
    //private Transform CheckForPowerup()
    //{
    //    if (GameObject.FindGameObjectWithTag("Powerup") != null)
    //    {
    //        return GameObject.FindGameObjectWithTag("Powerup").transform;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    private void DetectFire()
    {
        RapidFireCheck();
        SpreadFireCheck();

        Vector2 clickPoint;
        if (Input.GetMouseButton(0))
        {
            clickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            FireShot(clickPoint);
        }
    }

    private void RapidFireCheck()
    {
        if (rapidFireOn == true && rapidFireTimer > 0)
        {
            rapidFireTimer -= Time.deltaTime;
        }
        else if (rapidFireOn == true && rapidFireTimer < 0)
        {
            rapidFireOn = false;
            rapidFireTimer = 10.0f;
        }
    }

    private void SpreadFireCheck()
    {
        if (spreadShotOn == true && spreadShotTimer > 0)
        {
            spreadShotTimer -= Time.deltaTime;
        }
        else if (spreadShotOn == true && spreadShotTimer < 0)
        {
            spreadShotOn = false;
            spreadShotTimer = 10.0f;
            shotNumber = 1;
        }
    }

    private void FireShot(Vector2 target)
    {
        if (shotTimer < 0)
        {
            for (int i = 0; i < shotNumber; i++)
            {
                Vector3 spawnAt = transform.position;

                if (i == 1) { spawnAt.x += 0.3f; target.x += 0.6f; }
                if (i == 2) { spawnAt.x -= 0.3f; target.x -= 0.6f; }

                GameObject newShot = Instantiate(shot,
                                                 transform.position + (transform.up * 0.25f),
                                                 Quaternion.identity);
                ShotDirection sDir = newShot.GetComponent<ShotDirection>();
                sDir.destination = target;
                sDir.pORe = "p";
            }
            if (rapidFireOn) { shotTimer = 0.25f; } else { shotTimer = 1.0f; }
        }
    }

    private void SetGhost()
    {
        ghostPos.x = myPos.x;
        ghostPos.y = -1 * myPos.y;
        newGhostPos = ghostPos;

        ghost.position = Vector2.MoveTowards(ghostPos, newGhostPos, movementMultiplier * Time.deltaTime);

        enemySteps.Add(stepIndex, newGhostPos);
        stepIndex++;
    }
}
