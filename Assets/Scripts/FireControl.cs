using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireControl : MonoBehaviour
{
    // Game manager entity, script, and other entities
    private GameObject manager;
    private ManagerScript managerScript;
    private GameObject player;
    private Transform powerup;

    // Entity attributes
    private float coolDownTimer;

    // Movement attributes
    private float moveTimer;
    private Dictionary<int, Vector2> mySteps;
    private int stepCounter;

    // Weapon attributes and modifiers
    [HideInInspector] public GameObject shot; // Leave open for easy swapping of shots in dev mode    
    [HideInInspector] public float shotTimer; // Accessed by RFManager/SPManager
    [HideInInspector] public int shotNumber; // Accessed by SPManager
    private float fireChance;
    private float _fireChance;
    private float fireChanceTimer;
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
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (manager.GetComponent<ManagerScript>().gameOn && coolDownTimer > 0) 
        { 
            coolDownTimer -= Time.deltaTime; 
        }
        else
        {
            MovementCheck();
            //DetectFire();
        }
    }

    private void LoadPlaySettings()
    {
        // Manager and player settings
        manager = GameObject.FindGameObjectWithTag("GameController");
        managerScript = manager.GetComponent<ManagerScript>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Entity settings
        coolDownTimer = 2.0f;

        // Movement settings
        moveTimer = Random.Range(0.5f, 3.0f);
        //moveTimer = 5.0f;
        
        
        mySteps = new Dictionary<int, Vector2>();
        //mySteps = new Dictionary<int, Vector2>();
        stepCounter = 0;
        
        // Firing controls
        shotTimer = 1.0f;
        shotNumber = 1;
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
        if (managerScript.gameOn && coolDownTimer < 0)
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
        mySteps = managerScript.moveList;
        Debug.Log($"mSTEP: {mySteps.Count}");
        Debug.Log($"mSTEP: {mySteps.ContainsKey(stepCounter)}");
        if (mySteps.ContainsKey(stepCounter) && mySteps.Count >= 1)
        {
            Vector2 myPos = GetComponent<Rigidbody2D>().transform.position;
            Vector2 direction;
            powerup = CheckForPowerup();
            if (powerup != null)
            {
                if (powerup.position.y >= 0.5f)
                {
                    //direction = new Vector2(myPos.x - powerup.position.x, myPos.y - powerup.position.y).normalized;
                    //GetComponent<Rigidbody2D>().velocity = -1 * 5 * direction;


                    
                    Vector2 destination = new Vector2(powerup.position.x, powerup.position.y);

                    Vector2 currentPos = GetComponent<Rigidbody2D>().transform.position;
                    transform.position = Vector2.MoveTowards(currentPos, destination, Time.deltaTime);

                    Debug.Log($"MMM [{currentPos}]:[{destination}]");
                }
            }
            else
            {
                direction = new Vector2(myPos.x - mySteps[stepCounter].x, myPos.y - mySteps[stepCounter].y).normalized;
                GetComponent<Rigidbody2D>().velocity = -1 * 5 * direction;

                //Vector2 currentPos = GetComponent<Rigidbody2D>().transform.position;
                //Vector2 destination = new Vector2(mySteps[stepCounter].x, mySteps[stepCounter].y);
                //transform.position = Vector2.MoveTowards(currentPos, destination, 5 * Time.deltaTime);

                //Debug.Log($"MMM [{currentPos}]:[{destination}]");
            }
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
            newShot.GetComponent<ShotDirection>().destination = target;
            newShot.GetComponent<ShotDirection>().pORe = "e";
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
