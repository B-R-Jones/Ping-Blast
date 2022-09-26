using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementMultiplier;
    public float shiftSpeed;
    public bool fireSignal;
    public bool _CheckKeys;

    public Rigidbody2D enemy;
    public Rigidbody2D ghost;
    public GameObject shot;
    public GameObject manager;

    private Vector2 pos;
    private Vector2 newPos;
    private Vector2 ghostPos;
    private Vector2 newGhostPos;
    private Vector2 enemyPos;

    [HideInInspector]
    public Dictionary<int, Vector2> enemySteps;
    private int stepCounter;
    private int stepIndex;

    [HideInInspector]
    public int shotNumber;
    //[HideInInspector]
    public bool rapidFireOn;
    public bool spreadShotOn;
    public float rapidFireTimer;
    public float spreadShotTimer;

    private float enemyMoveTimer;

    //[HideInInspector]
    public float shotTimer;

    private bool enemyDetected;

    // Start is called before the first frame update
    void Start()
    {
        LoadPlaySettings();
    }

    private void LoadPlaySettings()
    {
        // Manager settings
        manager = GameObject.FindGameObjectWithTag("GameController");
        
        // Ghost settings
        ghost = GameObject.FindGameObjectWithTag("Ghost").gameObject.GetComponent<Rigidbody2D>();

        //Enemy and enemy movement
        enemy = GameObject.FindGameObjectWithTag("Enemy").gameObject.GetComponent<Rigidbody2D>();
        enemySteps = manager.GetComponent<ManagerScript>().moveList;
        enemySteps = new Dictionary<int, Vector2>();
        stepCounter = manager.GetComponent<ManagerScript>().moveCounter;
        stepIndex = manager.GetComponent<ManagerScript>().moveIndex;
        enemyMoveTimer = Random.Range(0f, 3f);

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

    // Update is called once per frame
    void FixedUpdate()
    {

        if (manager.GetComponent<ManagerScript>().gameOn)
        {
            DetectEnemy();
            SetPositions();
            MoveSquare();
            StopCheck();
            DetectFire();
            SetGhost();

            enemyMoveTimer -= Time.deltaTime;
            shotTimer -= Time.deltaTime;
            if (enemyMoveTimer < 0 && enemyDetected)
            {
                MoveSquareEnemy();
            }
        }
    }

    private void DetectEnemy()
    {
        if (enemy != null)
        {
            enemyDetected = true;
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("Enemy") != null)
            {
                enemy = GameObject.FindGameObjectWithTag("Enemy").gameObject.GetComponent<Rigidbody2D>();
            }
            enemyDetected = false;
        }
    }

    private void SetPositions()
    {
        pos = transform.position;
        if (enemyDetected) { enemyPos = enemy.position; }
        newPos = GetMovement(pos);
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

    private void StopCheck()
    {
        _CheckKeys = CheckKeys();
    }

    private bool CheckKeys()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MoveSquare()
    {
        Vector2 direction = new Vector2(GetComponent<Rigidbody2D>().position.x - newPos.x, GetComponent<Rigidbody2D>().position.y - newPos.y).normalized;
        GetComponent<Rigidbody2D>().velocity = -1 * movementMultiplier * shiftSpeed * direction;
    }

    private void MoveSquareEnemy()
    {
        if (enemySteps.ContainsKey(stepCounter) && enemySteps.Count >= 1)
        {
            enemyPos = enemy.position;
            if (CheckForPowerup() != null)
            {
                if (CheckForPowerup().position.y >= 0.5f)
                {
                    enemy.position = Vector2.MoveTowards(enemyPos, CheckForPowerup().position, movementMultiplier * Time.deltaTime);
                }
            }
            else
            {
                enemy.position = Vector2.MoveTowards(enemyPos, enemySteps[stepCounter], movementMultiplier * Time.deltaTime);
            }
        }
        stepCounter++;
        enemySteps.Remove(stepCounter - 1);
    }

    private Transform CheckForPowerup()
    {
        if (GameObject.FindGameObjectWithTag("Powerup") != null)
        {
            return GameObject.FindGameObjectWithTag("Powerup").transform;
        }
        else
        {
            return null;
        }
    }

    private void DetectFire()
    {

        RapidFireCheck();

        SpreadFireCheck();

        Vector2 clickPoint;
        if (Input.GetMouseButton(0)) 
        {
                fireSignal = true;

                clickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                FireShot(clickPoint);
        } 
        else 
        { 
            fireSignal = false; 
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

    void FireShot(Vector2 target)
    {
        if (shotTimer < 0) 
        {
            for (int i = 0; i < shotNumber; i++)
            {
                Vector3 spawnAt = transform.position;
                if (i == 1) { spawnAt.x += 0.3f; target.x += 0.6f; }
                if (i == 2) { spawnAt.x -= 0.3f; target.x -= 0.6f; }
                var newShot = Instantiate(shot, transform.position + (transform.up * 0.25f),Quaternion.identity); 
                newShot.GetComponent<ShotDirection>().destination = target;
                newShot.GetComponent<ShotDirection>().pORe = "p";
            }
            if (rapidFireOn) { shotTimer = 0.25f; } else { shotTimer = 1.0f; }
        }
    }

    private void SetGhost()
    {
        ghostPos.x = pos.x;
        ghostPos.y = -1 * pos.y;
        newGhostPos = ghostPos;

        ghost.position = Vector2.MoveTowards(ghostPos, newGhostPos, movementMultiplier * Time.deltaTime);

        enemySteps.Add(stepIndex, newGhostPos);
        stepIndex++;
    }
}
