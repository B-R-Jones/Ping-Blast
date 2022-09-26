using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireControl : MonoBehaviour
{
    public float fireChance;
    public float _fireChance;
    public GameObject shot;
    public float fireChanceTimer;
    public float shotTimer;
    public GameObject manager;

    public int shotNumber;
    public bool rapidFireOn;
    public bool spreadShotOn;
    public float rapidFireTimer;
    public float spreadShotTimer;

    // Start is called before the first frame update
    void Start()
    {
        fireChanceTimer = 15.0f;
        shotTimer = 1.0f;
        rapidFireTimer = 10.0f;
        spreadShotTimer = 10.0f;
        manager = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        DetectFire();

    }

    private void DetectFire()
    {

        if (manager.GetComponent<ManagerScript>().gameOn)
        {
            _fireChance = Random.Range(0.0f, 100.0f);
            fireChanceTimer -= Time.deltaTime;
            shotTimer -= Time.deltaTime;

            RapidFireCheck();
            SpreadFireCheck();

            if (shotTimer < 0)
            {
                if (fireChance > _fireChance) 
                { 
                    FireShot(GameObject.FindGameObjectWithTag("Player").transform.position); 
                }
            }

            if (fireChanceTimer < 0)
            {
                fireChanceTimer = 15.0f;
                fireChance = Random.Range(0.0f, 100.0f);
            }
        }
    }

    void FireShot(Vector2 target)
    {
        Debug.Log($"SHOTS: {shotNumber}");
        for (int i = 0; i < shotNumber; i++)
        {
            Vector3 spawnAt = transform.position;
            if (i == 1) { spawnAt.x += 0.3f; target.x += 0.6f; }
            if (i == 2) { spawnAt.x -= 0.3f; target.x -= 0.6f; }
            var newShot = Instantiate(shot, (transform.position + (0.25f * -1 * transform.up)), Quaternion.identity);
            newShot.GetComponent<ShotDirection>().destination = target;
            newShot.GetComponent<ShotDirection>().pORe = "e";
            Debug.Log($"FIR: ({i}) : [{target}] : [{newShot.GetComponent<ShotDirection>().transform.position}] : {newShot.GetComponent<ShotDirection>().pORe}");
        }

        if (rapidFireOn) { shotTimer = 0.25f; } else { shotTimer = 1.0f; }
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
