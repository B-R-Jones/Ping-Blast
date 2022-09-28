using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    // Game entities and attributes
    private GameObject player;
    private Transform playerSpawn;
    private GameObject enemy; // Investigate if needed after player_controller branch
    private Rigidbody2D ghost;
    public GameObject shot; // Leave open for easy swapping of ammo in the future

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
    public float respawnEnemyTimer;
    public bool playerAlive;

    [HideInInspector]
    public int score;
    [HideInInspector]
    public int scoreMultiplier;
    public TextMeshPro scoreboard;
    public TextMeshPro multiplierBoard;
    public TextMeshPro nextHitBoard;
    private int scoreFrames;
    public int lastScore;



    public float spawnPowerupTimer;
    public float spawnPowerupChance;
    public float spawnPowerupLocationCode;
    public float spawnPowerupPickupCode;
    public float spawnPowerupLocationX;
    public float spawnPowerupLocationY;

    public GameObject RapidfirePowerup;
    public GameObject SpreadshotPowerup;

    public bool gameOn;
    public bool moveCamera;
    public Vector3 camPosMenu;
    public Vector3 camPosField;

    public Camera mainCamera;


    // Start is called before the first frame update
    void Start()
    {
        SetPlayField();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //if (moveCamera) { MoveCameraTo(0); } else { MoveCameraTo(1); }
        if (gameOn)
        {
            MoveCameraTo(0);

            FrameScore();

            PowerupCheck();

            EnemyCheck();

            PlayerCheck();
        }
        else
        {
            MoveCameraTo(1);
        }
    }

    private void PlayerCheck()
    {
        if (playerAlive)
        {
            multiplierBoard.text = scoreMultiplier.ToString() + "x";
            scoreboard.text = score.ToString();
            nextHitBoard.text = "NEXT HIT: " + (50 * scoreMultiplier).ToString();
        }
        else
        {
            gameOn = false;
            MoveCameraTo(1);
            SetPlayField();
        }
    }

    private void EnemyCheck()
    {
        if (!enemyAlive)
        {
            if (respawnEnemyTimer < 0)
            {
                GameObject newEnemy = Instantiate(enemy);

                // Move to our spawn location
                float spawnEnemyAtX = Random.Range(-4.5f, 4.5f);
                float spawnEnemyAtY = Random.Range(0.5f, 9.5f);
                Vector3 spawnLocation = new Vector3(spawnEnemyAtX, spawnEnemyAtY, 1.0f);
                newEnemy.transform.SetPositionAndRotation(spawnLocation, Quaternion.identity);

                // Set new enemy for player
                player.GetComponent<PlayerController>().enemy = newEnemy.GetComponent<Rigidbody2D>();
                enemyAlive = true;
                respawnEnemyTimer = 3.0f;
            }
            else
            {
                respawnEnemyTimer -= Time.deltaTime;
            }
        }
    }

    private void FrameScore()
    {
        if (scoreFrames == 60) { score += 1 * scoreMultiplier; scoreFrames = 0; } else { scoreFrames += 1; }
    }

    private void SetPlayField()
    {
        // Player settings
        PlayerCheck();

        // Camera settings
        SetCameraStateAndPositions();

        // Enemy settings
        SetEnemyStateAndMoveList();

        // Score settings
        SetGameStateAndScoreboard();
    }

    private void PlayerCheck()
    {
        playerAlive = GameObject.FindGameObjectWithTag("Player");
        if (!playerAlive)
        {
            Instantiate(player, playerSpawn);
            playerAlive = true;
        }
        else
        {
            playerSpawn = player.transform;
        }
    }

    private void SetCameraStateAndPositions()
    {
        moveCamera = false;
        camPosMenu = new(-40.0f, 0.0f, -1.0f);
        camPosField = new(0.0f, 0.0f, -1.0f);
    }

    private void SetEnemyStateAndMoveList()
    {
        enemyAlive = true;
        moveList = new Dictionary<int, Vector2>();
        respawnEnemyTimer = 3.0f;
    }

    private void SetScoreAttributes()
    {
        lastScore = score;
        score = 0;
        scoreFrames = 0;
        scoreMultiplier = 1;
    }

    private void SetGameStateAndScoreboard()
    {
        gameOn = false;
        scoreboard.text = score.ToString();
        spawnPowerupTimer = 10.0f;
    }

    private void PowerupCheck()
    {
        if (spawnPowerupTimer < 0)
        {
            spawnPowerupChance = Random.Range(0.0f, 100.0f);
            if (spawnPowerupChance > 20.0f)
            {
                //setup our powerup details
                Transform spawnPowerupLocation;
                GameObject spawnPowerupPickup;

                //find our spawn location
                spawnPowerupLocationCode = Random.Range(0.0f, 100.0f);

                if (spawnPowerupLocationCode > 50.0f)
                {
                    spawnPowerupLocationX = Random.Range(-4.5f, 4.5f);
                    spawnPowerupLocationY = Random.Range(-0.5f, -9.5f);
                }
                else if(spawnPowerupLocationCode < 50.0f)
                {
                    spawnPowerupLocationX = Random.Range(-4.5f, 4.5f);
                    spawnPowerupLocationY = Random.Range(0.5f, 9.5f);
                }
                spawnPowerupLocation = transform;
                spawnPowerupLocation.position.Set(spawnPowerupLocationX, spawnPowerupLocationY, 1.0f);

                //decide which powerup to spawn
                spawnPowerupPickupCode = Random.Range(0.0f, 100.0f);

                if (spawnPowerupPickupCode > 50.0)
                {
                    spawnPowerupPickup = RapidfirePowerup;
                }
                else
                {
                    spawnPowerupPickup = SpreadshotPowerup;
                }


                GameObject spawnedPickup = Instantiate(spawnPowerupPickup);
                spawnedPickup.transform.SetPositionAndRotation(new(spawnPowerupLocationX, spawnPowerupLocationY, 1.0f), Quaternion.identity);

                spawnPowerupTimer = 5.0f;

            }

        }
        else
        {
            spawnPowerupTimer -= Time.deltaTime;
        }
    }

    public void MoveCameraTo(int mode)
    {
        Debug.Log($"Moving camera...{mode}");
        switch (mode)
        {
            case 0:
                //move toward field
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camPosField, 2.0f * Time.deltaTime);
                break;
            case 1:
                //move away from field
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camPosMenu, 2.0f * Time.deltaTime);
                break;
            default:
                //move away from field
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camPosMenu, 2.0f * Time.deltaTime);
                break;
        }        
    }

    public void BeginGame()
    {
        moveCamera = true;
        gameOn = true;
    }
}
