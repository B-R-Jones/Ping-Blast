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
    //private ManagerScript managerScript;
    private ManagerController managerController;
    public Rigidbody2D ghost; // Accessed by ManagerController

    // Vector positions of the ghost
    private Vector2 ghostPos;
    private Vector2 newGhostPos;

    // Enemy movement dictionary, steppers, etc.
    private Dictionary<int, Vector2> enemySteps;
    public int stepIndex;

    // Weapon attributes and modifiers
    public GameObject shot; // Leave open for easy swapping of shots in dev mode
    public int shotNumber; // Accessed by SPManager
    public float shotTimer; // Accessed by RFManager
    public bool rapidFireOn; // Accessed by RFManager
    public bool spreadShotOn; // Accessed by SPManager
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
        if (managerController.gameOn)
        {
            SetPositions();
            MoveSquare();
            DetectFire();
            SetGhost();
            shotTimer -= Time.deltaTime;
        }
    }

    private void LoadPlaySettings()
    {
        // Manager settings
        manager = GameObject.FindGameObjectWithTag("GameController");
        managerController = manager.GetComponent<ManagerController>();

        // Player settings
        myRigidbody = GetComponent<Rigidbody2D>();
        myPos = myRigidbody.transform.position;

        // Ghost settings
        ghost = GameObject.FindGameObjectWithTag("Ghost").gameObject.GetComponent<Rigidbody2D>();

        //Enemy movement list
        enemySteps = managerController.moveList;
        enemySteps = new Dictionary<int, Vector2>();
        stepIndex = managerController.moveIndex;

        // Powerup and firing settings
        rapidFireOn = false;
        rapidFireTimer = 10.0f;
        spreadShotOn = false;
        spreadShotTimer = 10.0f;
        shotTimer = 1.0f;
        shotNumber = 1;

        // Movement settings
        if (movementMultiplier == 0) { movementMultiplier = 5; }
        shiftSpeed = 0.0f;
    }

    private void SetPositions()
    {
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
                BulletController bCon = newShot.GetComponent<BulletController>();
                bCon.destination = target;
                bCon.pORe = "p";
            }
            if (rapidFireOn) { shotTimer = 0.25f; } else { shotTimer = 1.0f; }
        }
    }

    private void SetGhost()
    {
        myPos = myRigidbody.transform.position;
        ghostPos.x = myPos.x;
        ghostPos.y = -1 * myPos.y;
        newGhostPos = ghostPos;

        ghost.position = Vector2.MoveTowards(ghostPos, newGhostPos, movementMultiplier * Time.deltaTime);

        if (newGhostPos == null) { Debug.Log("navi"); }
        enemySteps.Add(stepIndex, newGhostPos);
        managerController.moveList = enemySteps;
        managerController.moveIndex = stepIndex;
        stepIndex++;
    }
}
