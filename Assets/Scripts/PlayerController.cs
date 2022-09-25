using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int movementMultiplier;
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

    private float timer;
    private float shotTimer;

    private bool enemyDetected;

    // Start is called before the first frame update
    void Start()
    {
        //enemySteps = new Dictionary<int, Vector2>();
        enemySteps = manager.GetComponent<ManagerScript>().moveList;
        enemySteps = new Dictionary<int, Vector2>();
        stepCounter = manager.GetComponent<ManagerScript>().moveCounter;
        stepCounter = 0;
        stepIndex = manager.GetComponent<ManagerScript>().moveIndex;
        stepIndex = 0;

        timer = Random.Range(0f, 3f);
        shotTimer = 2.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DetectEnemy();
        SetPositions();
        MoveSquare();
        StopCheck();
        DetectFire();
        SetGhost();

        timer -= Time.deltaTime;
        shotTimer -= Time.deltaTime;
        if (timer < 0 && enemyDetected)
        {
            MoveSquareEnemy();
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
        GetComponent<Rigidbody2D>().velocity = direction * -1 * movementMultiplier;
    }

    private void MoveSquareEnemy()
    {
        if (enemySteps.ContainsKey(stepCounter) && enemySteps.Count >= 1)
        {
            Debug.Log($"MSE001 Processing Step: {enemySteps[stepCounter]}");
            enemyPos = enemy.position;
            enemy.position = Vector2.MoveTowards(enemyPos, enemySteps[stepCounter], movementMultiplier * Time.deltaTime);
        }
        stepCounter++;
        enemySteps.Remove(stepCounter - 1);
    }

    private void DetectFire()
    {
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

    void FireShot(Vector2 target)
    {
        if (shotTimer < 0) 
        {
            var newShot = Instantiate(shot, transform.position + (transform.up * 0.25f), transform.rotation); shotTimer = 2.0f;
            newShot.GetComponent<ShotDirection>().destination = target;
            newShot.GetComponent<ShotDirection>().pORe = "p";
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
