using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Game manager entity, script, and other entities
    private GameObject manager;
    private ManagerController managerController;
    private GameObject player;
    private Transform powerup;

    // Entity attributes
    private float coolDownTimer;

    // Movement attributes
    private float moveTimer;
    private Dictionary<int, Vector2> mySteps;
    public int stepCounter;

    // Weapon attributes and modifiers
    public GameObject shot; // Leave open for easy swapping of shots in dev mode    
    public float shotTimer; // Accessed by RFManager/SPManager
    public int shotNumber; // Accessed by SPManager
    private float fireChance;
    private float _fireChance;
    private float fireChanceTimer;
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
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (manager.GetComponent<ManagerController>().gameOn && coolDownTimer > 0) 
        { 
            coolDownTimer -= Time.deltaTime; 
        }
        else
        {
            MovementCheck();
            DetectFire();
        }
    }

    private void LoadPlaySettings()
    {
        // Manager and player settings
        manager = GameObject.FindGameObjectWithTag("GameController");
        managerController = manager.GetComponent<ManagerController>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Entity settings
        coolDownTimer = 0.25f;

        // Movement settings
        moveTimer = Random.Range(0.5f, 1.5f);
        
        mySteps = new Dictionary<int, Vector2>();
        stepCounter = managerController.moveIndex;
        
        // Firing controls
        shotTimer = 1.0f;
        shotNumber = 1;
        fireChance = Random.Range(0.0f, 100.0f);
        fireChanceTimer = 15.0f;
        rapidFireTimer = 10.0f;
        spreadShotTimer = 10.0f;
    }

    private void MovementCheck()
    {
        if (moveTimer < 0) { MoveSquareEnemy(); } else { moveTimer -= Time.deltaTime; }
    }

    private void DetectFire()
    {
        if (managerController.gameOn && coolDownTimer < 0)
        {
            _fireChance = Random.Range(0.0f, 100.0f);
            fireChanceTimer -= Time.deltaTime;
            shotTimer -= Time.deltaTime;

            RapidFireCheck();
            SpreadFireCheck();

            if (shotTimer < 0)
            {
                if (fireChance > _fireChance) { FireShot(player.transform.position); }
                if (rapidFireOn) { shotTimer = 0.25f; } else { shotTimer = 1.0f; }
            }

            if (fireChanceTimer < 0)
            {
                fireChanceTimer = 15.0f;
                fireChance = Random.Range(0.0f, 100.0f);
            }
        }
    }

    private void MoveSquareEnemy()
    {
        mySteps = managerController.moveList;
        
        if (mySteps == null) { Debug.Log("STEPS NULL"); return; }
        if (mySteps.ContainsKey(stepCounter) && mySteps.Count >= 1)
        {
            Vector2 currentPos = GetComponent<Rigidbody2D>().transform.position;
            Vector2 destination;
            powerup = CheckForPowerup();
            if (powerup != null)
            {
                if (powerup.position.y >= 0.5f)
                {
                    destination = powerup.position;
                    transform.position = Vector2.MoveTowards(currentPos, destination, 5 * Time.deltaTime);
                }
            }
            else
            {
                destination = mySteps[stepCounter];
                transform.position = Vector2.MoveTowards(currentPos, destination, 5 * Time.deltaTime);
            }
        }
        else if (stepCounter > managerController.moveIndex)
        {
            stepCounter = managerController.moveCounter - 1;
        }
        stepCounter++;
        mySteps.Remove(stepCounter - 1);
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

    void FireShot(Vector2 target)
    {
        for (int i = 0; i < shotNumber; i++)
        {
            Vector3 spawnAt = transform.position;
            if (i == 1) { spawnAt.x += 0.3f; target.x += 0.6f; }
            if (i == 2) { spawnAt.x -= 0.3f; target.x -= 0.6f; }
            var newShot = Instantiate(shot, (transform.position + (0.25f * -1 * transform.up)), Quaternion.identity);
            newShot.GetComponent<BulletController>().destination = target;
            newShot.GetComponent<BulletController>().pORe = "e";
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
}
