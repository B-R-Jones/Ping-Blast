using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManagerController : MonoBehaviour
{
    // Enemy state, movement dictionary, steppers, respawn timer and shot
    private GameObject enemy; // Leave open to easily swap out enemies?
    public GameObject enemyToSpawn;
    public bool enemyAlive;
    public Dictionary<int, Vector2> moveList; // Accessed by PlayerController/FireControl
    public int moveCounter; // Accessed by FireControl
    public int moveIndex; // Accessed by PlayerController/FireControl
    private float respawnEnemyTimer;
    public float respawnEnemySeconds; // Leave open to easily change in the future
    public GameObject shot; // Leave open for easy swapping of ammo in the future

    // Player entity, state, spawn and ghost
    private GameObject player;
    public GameObject playerToSpawn;
    public bool playerAlive;
    private Transform playerSpawn;

    // Scoreboard entities and attributes
    public TextMeshPro scoreboard;
    public TextMeshPro multiplierBoard;
    public TextMeshPro nextHitBoard;
    public int score; // Accessed by RFManager/SPManager
    public int scoreMultiplier; // Accessed by RFManager/SPManager
    private int scoreFrames;
    public int lastScore;

    // Powerup entities and spawning numbers and attributes
    public GameObject RapidfirePowerup; // Leave open for easy swapping of powerups in the future
    public GameObject SpreadshotPowerup; // Leave open for easy swapping of powerups in the future
    private float spawnPowerupTimer;
    private float spawnPowerupChance;
    private float spawnPowerupLocationCode;
    private float spawnPowerupPickupCode;
    private float spawnPowerupLocationX;
    private float spawnPowerupLocationY;

    // Camera, state and attributes
    public Camera mainCamera; // Leave open for easy swapping of cameras in the future
    private bool moveCamera;
    private Vector3 camPosMenu;
    private Vector3 camPosField;

    // Game state
    public bool gameOn; // Accessed by multiple scripts to check game state

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
        if (gameOn)
        {
            EnemyCheck();
            MoveCameraTo(0);
            FrameScore();
            PowerupCheck();
            PlayerCheck();
        }
        else
        {
            MoveCameraTo(1);
        }
    }

    private void PlayerCheck()
    {
        PlayerAliveCheck();
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
            ClearField();
            SetPlayField();
        }
    }

    private void PlayerAliveCheck()
    {
        playerAlive = GameObject.FindGameObjectWithTag("Player");
        if (!playerAlive)
        {
            Instantiate(playerToSpawn, playerSpawn);
            player = GameObject.FindGameObjectWithTag("Player");
            //playerAlive = true;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerSpawn = player.transform;
        }
    }

    private void EnemyCheck()
    {
        enemyAlive = GameObject.FindGameObjectWithTag("Enemy");
        if (!enemyAlive)
        {
            Debug.Log($"No enemy found!");
            if (respawnEnemyTimer < 0) { RespawnEnemy(1); } else { respawnEnemyTimer = DecayTimer(respawnEnemyTimer); }
        }
        else
        {
            enemy = GameObject.FindGameObjectWithTag("Enemy");
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

    private void SetCameraStateAndPositions()
    {
        moveCamera = false;
        camPosMenu = new(-40.0f, 0.0f, -1.0f);
        camPosField = new(0.0f, 0.0f, -1.0f);
    }

    private void SetEnemyStateAndMoveList()
    {
        RespawnEnemy(0);
        moveList = new Dictionary<int, Vector2>();
        respawnEnemyTimer = respawnEnemySeconds < 0.5f ? 3.0f : respawnEnemySeconds;
    }

    private void SetGameStateAndScoreboard()
    {
        SetScoreAttributes();
        gameOn = false;
        scoreboard.text = score.ToString();
        spawnPowerupTimer = 10.0f;
    }

    private void SetScoreAttributes()
    {
        lastScore = score;
        score = 0;
        scoreFrames = 0;
        scoreMultiplier = 1;
    }

    private void RespawnEnemy(int mode)
    {
        //GameObject newEnemy = Instantiate(enemy);
        enemy = Instantiate(enemyToSpawn);
        float spawnEnemyAtX = 0.0f;
        float spawnEnemyAtY = 0.0f;
        switch (mode)
        {
            case 0:
                // fresh start
                spawnEnemyAtX = 0.0f;
                spawnEnemyAtY = 8.0f;
                break;
            case 1:
                // enemy has been killed and needs respawning
                spawnEnemyAtX = Random.Range(-4.5f, 4.5f);
                spawnEnemyAtY = Random.Range(0.5f, 9.5f);
                break;
        }
        Vector3 spawnLocation = new Vector3(spawnEnemyAtX, spawnEnemyAtY, 1.0f);
        enemy.transform.SetPositionAndRotation(spawnLocation, Quaternion.identity);
        enemyAlive = true;
        respawnEnemyTimer = respawnEnemySeconds;
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
            spawnPowerupTimer = DecayTimer(spawnPowerupTimer);
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

    private float DecayTimer(float timer)
    {
        return timer -= Time.deltaTime;
    }

    private void ClearField()
    {
        Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        Destroy(GameObject.FindGameObjectWithTag("Powerup"));
        Destroy(GameObject.FindGameObjectWithTag("Bullet"));
    }
}
