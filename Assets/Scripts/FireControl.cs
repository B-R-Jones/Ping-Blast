using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireControl : MonoBehaviour
{
    public float fireChance;
    public GameObject shot;
    private bool changeFireChance;
    private float fireChanceTimer;
    private float shotTimer;
    // Start is called before the first frame update
    void Start()
    {
        changeFireChance = false;
        fireChanceTimer = 15.0f;
        shotTimer = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float _fireChance = Random.Range(0.0f, 100.0f);
        fireChanceTimer -= Time.deltaTime;
        shotTimer -= Time.deltaTime;


        fireChance = Random.Range(0.0f, 100.0f);

        if (shotTimer < 0)
        {
            if (fireChance > _fireChance) { FireShot(GameObject.FindGameObjectWithTag("Player").transform.position); }
            shotTimer = 1.0f;
        }

        if (fireChanceTimer < 0) 
        { 
            fireChanceTimer = 5.0f;
            fireChance = Random.Range(0.0f, 100.0f);
        }
    }

    void FireShot(Vector2 target)
    {
        if (shotTimer < 0)
        {
            shotTimer = 1.0f;
            var newShot = Instantiate(shot, transform.position + (transform.up * -1 * 0.25f), transform.rotation);
            newShot.GetComponent<ShotDirection>().destination = target;
            newShot.GetComponent<ShotDirection>().pORe = "e";
        }
    }
}
